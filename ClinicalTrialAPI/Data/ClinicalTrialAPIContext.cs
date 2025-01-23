using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicalTrialAPI.Models;

namespace ClinicalTrialAPI.Data
{
    public class ClinicalTrialAPIContext : DbContext
    {
        public ClinicalTrialAPIContext (DbContextOptions<ClinicalTrialAPIContext> options)
            : base(options)
        {
        }

        public DbSet<ClinicalTrialAPI.Models.ClinicalTrialMetadata> ClinicalTrialMetadata { get; set; } = default!;
    }
}
