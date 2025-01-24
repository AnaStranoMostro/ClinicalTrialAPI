using ClinicalTrialAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialAPI.Data
{
    public class ClinicalTrialRepository
    {
        private readonly ClinicalTrialAPIContext _context;

        public ClinicalTrialRepository(ClinicalTrialAPIContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClinicalTrialMetadata>> GetClinicalTrialMetadataAsync(string? trialId, string? title, ClinicalTrialStatus? status)
        {
            IQueryable<ClinicalTrialMetadata> query = _context.ClinicalTrialMetadata.AsQueryable();

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

        public async Task<ClinicalTrialMetadata?> GetClinicalTrialMetadataByIdAsync(string id)
        {
            return await _context.ClinicalTrialMetadata.FindAsync(id);
        }

        public async Task AddClinicalTrialMetadataAsync(ClinicalTrialMetadata clinicalTrialMetadata)
        {
             _context.ClinicalTrialMetadata.Add(clinicalTrialMetadata);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteClinicalTrialMetadataAsync(ClinicalTrialMetadata clinicalTrialMetadata)
        {
             _context.ClinicalTrialMetadata.Remove(clinicalTrialMetadata);
             await _context.SaveChangesAsync();
        }

        public bool ClinicalTrialMetadataExists(string id)
        {
            return _context.ClinicalTrialMetadata.Any(e => e.TrialId == id);
        }
    }
}
