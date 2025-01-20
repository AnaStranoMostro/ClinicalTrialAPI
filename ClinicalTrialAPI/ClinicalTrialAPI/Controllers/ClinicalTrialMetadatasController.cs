using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialAPI.Data;
using ClinicalTrialAPI.Models;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace ClinicalTrialAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicalTrialMetadatasController : ControllerBase
    {
        private readonly ClinicalTrialAPIContext _context;

        public ClinicalTrialAPIContext Context => _context;

        public ClinicalTrialMetadatasController(ClinicalTrialAPIContext context)
        {
            _context = context;
        }

        // GET: api/ClinicalTrialMetadatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicalTrialMetadata>>> GetClinicalTrialMetadata()
        {
            return await Context.ClinicalTrialMetadata.ToListAsync();
        }

        // GET: api/ClinicalTrialMetadatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicalTrialMetadata>> GetClinicalTrialMetadata(string id)
        {
            var clinicalTrialMetadata = await Context.ClinicalTrialMetadata.FindAsync(id);

            if (clinicalTrialMetadata == null)
            {
                return NotFound();
            }

            return clinicalTrialMetadata;
        }

        // PUT: api/ClinicalTrialMetadatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinicalTrialMetadata(string id, ClinicalTrialMetadata clinicalTrialMetadata)
        {
            if (id != clinicalTrialMetadata.TrialId)
            {
                return BadRequest();
            }

            Context.Entry(clinicalTrialMetadata).State = EntityState.Modified;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicalTrialMetadataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ClinicalTrialMetadata
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClinicalTrialMetadata>> PostClinicalTrialMetadata(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            string jsonContent;
            using (var reader = new StreamReader(jsonFile.OpenReadStream()))
            {
                jsonContent = await reader.ReadToEndAsync();
            }

            var schema = JSchema.Parse(@"{
                '$schema': 'http://json-schema.org/draft-07/schema#',
                'title': 'ClinicalTrialMetadata',
                'type': 'object',
                'properties': {
                    'trialId': {
                        'type': 'string'
                    },
                    'title': {
                        'type': 'string'
                    },
                    'startDate': {
                        'type': 'string',
                        'format': 'date'
                    },
                    'endDate': {
                        'type': 'string',
                        'format': 'date'
                    },
                    'participants': {
                        'type': 'integer',
                        'minimum': 1
                    },
                    'status': {
                        'type': 'string',
                        'enum': [
                            'Not Started',
                            'Ongoing',
                            'Completed'
                        ]
                    }
                },
                'required': [
                    'trialId',
                    'title',
                    'startDate',
                    'status'
                ],
                'additionalProperties': false
            }");

            JObject jsonObject = JObject.Parse(jsonContent);
            if (!jsonObject.IsValid(schema, out IList<string> validationErrors))
            {
                return BadRequest(new { Errors = validationErrors });
            }

            var clinicalTrialMetadata = jsonObject.ToObject<ClinicalTrialMetadata>();

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

        // DELETE: api/ClinicalTrialMetadatas/5
        [HttpDelete("{id}")]
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
