using ClinicalTrialAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrialAPI.Data
{
    /// <summary>
    /// Database context for Clinical Trial API.
    /// </summary>
    public class ClinicalTrialAPIContext : DbContext
    {
        public ClinicalTrialAPIContext(DbContextOptions<ClinicalTrialAPIContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Clinical Trial Metadata.
        /// </summary>
        public DbSet<ClinicalTrialMetadata> ClinicalTrialMetadata { get; set; } = default!;
    }
}
