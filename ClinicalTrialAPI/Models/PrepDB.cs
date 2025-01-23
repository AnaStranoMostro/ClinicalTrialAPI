using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClinicalTrialAPI.Data;


namespace ClinicalTrialAPI.Models;

public static class PrepDB
{

    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<ClinicalTrialAPIContext>());
        }
    }

    public static void SeedData(ClinicalTrialAPIContext context)
    {
        System.Console.WriteLine("Applying Migrations...");
        context.Database.Migrate();

        if (!context.ClinicalTrialMetadata.Any())
        {
            System.Console.WriteLine("Adding data - seeding...");
            context.ClinicalTrialMetadata.AddRange(
                new ClinicalTrialMetadata()
                {
                    TrialId = "1",
                    Title = "COVID-19 Vaccine Trial",
                    StartDate = DateTime.Parse("2022-01-01"),
                    EndDate = DateTime.Parse("2022-12-31"),
                    Participants = 1000,
                    Status = ClinicalTrialStatus.NotStarted
                },
                new ClinicalTrialMetadata()
                {
                    TrialId = "2",
                    Title = "COVID-19 Treatment Trial",
                    StartDate = DateTime.Parse("2022-01-01"),
                    EndDate = DateTime.Parse("2022-12-31"),
                    Participants = 500,
                    Status = ClinicalTrialStatus.Ongoing
                }
            );
            context.SaveChanges();
        }
        else
        {
            System.Console.WriteLine("Already have data - not seeding");
        }
    }
}
