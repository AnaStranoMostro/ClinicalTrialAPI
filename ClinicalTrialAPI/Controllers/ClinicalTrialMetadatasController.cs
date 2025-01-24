using System.Reflection;
using ClinicalTrialAPI.Data;
using ClinicalTrialAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Swashbuckle.AspNetCore.Annotations;
using ClinicalTrialAPI.Helpers;


namespace ClinicalTrialAPI.Controllers
{
    /// <summary>
    /// Controller for managing clinical trial metadata.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialMetadatasController : ControllerBase
    {
        private readonly ClinicalTrialAPIContext _context;

        /// <summary>
        /// Gets the context for the clinical trial API.
        /// </summary>
        public ClinicalTrialAPIContext Context => _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClinicalTrialMetadatasController"/> class.
        /// </summary>
        /// <param name="context">The context for the clinical trial API.</param>
        public ClinicalTrialMetadatasController(ClinicalTrialAPIContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a list of clinical trial metadata.
        /// </summary>
        /// <param name="trialId">The trial ID to filter by.</param>
        /// <param name="title">The title to filter by.</param>
        /// <param name="status">The status to filter by.</param>
        /// <returns>A list of clinical trial metadata.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Gets a list of clinical trial metadata", Description = "Retrieves a list of clinical trial metadata based on optional filters.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the list of clinical trial metadata", typeof(IEnumerable<ClinicalTrialMetadata>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
        public async Task<ActionResult<IEnumerable<ClinicalTrialMetadata>>> GetClinicalTrialMetadata([FromQuery] string? trialId, [FromQuery] string? title, [FromQuery] ClinicalTrialStatus? status)
        {
            IQueryable<ClinicalTrialMetadata> query = Context.ClinicalTrialMetadata.AsQueryable();

            if (!string.IsNullOrEmpty(trialId))
            {
                query = query.Where(ct => ct.TrialId == trialId);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(ct => ct.Title.Contains(title));
            }

            if (status.HasValue)
            {
                query = query.Where(ct => ct.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Creates a new clinical trial metadata.
        /// </summary>
        /// <param name="jsonFile">The JSON file containing clinical trial metadata.</param>
        /// <returns>The created clinical trial metadata.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new clinical trial metadata", Description = "Creates a new clinical trial metadata from the provided JSON file.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Clinical trial metadata created successfully", typeof(ClinicalTrialMetadata))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Clinical trial metadata already exists")]
        public async Task<ActionResult<ClinicalTrialMetadata>> PostClinicalTrialMetadata(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            string jsonContent;
            //getting content of the file
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                jsonContent = await reader.ReadToEndAsync();
            }

            if (!JsonSchemaHelper.ValidateJson(jsonContent, out IList<string> validationErrors))
            {
                return BadRequest(new { Errors = validationErrors.ToList() });
            }

            var clinicalTrialMetadata = JsonSchemaHelper.ParseJson(jsonContent).ToObject<ClinicalTrialMetadata>();

            if (clinicalTrialMetadata == null)
            {
                return BadRequest("Invalid clinical trial metadata.");
            }

            // If end date is null or empty string, set it to 1 month after start date
            if (!clinicalTrialMetadata.EndDate.HasValue)
            {
                clinicalTrialMetadata.EndDate = clinicalTrialMetadata.StartDate.AddMonths(1);
                clinicalTrialMetadata.Status = ClinicalTrialStatus.Ongoing;
            }
            // Calculate trial duration and set status
            if (clinicalTrialMetadata.EndDate.HasValue)
            {
                clinicalTrialMetadata.TrialDuration = CalculateTrialDuration(clinicalTrialMetadata.StartDate, clinicalTrialMetadata.EndDate.Value);
            }
            else
            {
                clinicalTrialMetadata.TrialDuration = CalculateTrialDuration(clinicalTrialMetadata.StartDate, clinicalTrialMetadata.StartDate.AddMonths(1));
            }

            Context.ClinicalTrialMetadata.Add(clinicalTrialMetadata);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ClinicalTrialMetadataExists(clinicalTrialMetadata.TrialId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetClinicalTrialMetadata", new { id = clinicalTrialMetadata.TrialId }, clinicalTrialMetadata);
        }

        private int CalculateTrialDuration(DateTime startDate, DateTime endDate)
        {
  
             return (endDate - startDate).Days;
           
        }

        /// <summary>
        /// Deletes a clinical trial metadata.
        /// </summary>
        /// <param name="id">The trial ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a clinical trial metadata", Description = "Deletes the clinical trial metadata for the specified trial ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Clinical trial metadata deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Clinical trial metadata not found")]
        public async Task<IActionResult> DeleteClinicalTrialMetadata(string id)
        {
            var clinicalTrialMetadata = await Context.ClinicalTrialMetadata.FindAsync(id);
            if (clinicalTrialMetadata == null)
            {
                return NotFound();
            }

            Context.ClinicalTrialMetadata.Remove(clinicalTrialMetadata);
            await Context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicalTrialMetadataExists(string id)
        {
            return Context.ClinicalTrialMetadata.Any(e => e.TrialId == id);
        }
    }
}
