using ClinicalTrialAPI.Data;
using Microsoft.EntityFrameworkCore;


namespace ClinicalTrialAPI.Models;

public static class PrepDB
{

    public static void PrepPopulation(IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<ClinicalTrialAPIContext>();
        if (context != null)
        {
            SeedData(context);
        }
        else
        {
           Console.WriteLine("Error: ClinicalTrialAPIContext is null.");
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
                    Status = ClinicalTrialStatus.NotStarted,
                    TrialDuration = 365
                },
                new ClinicalTrialMetadata()
                {
                    TrialId = "2",
                    Title = "COVID-19 Treatment Trial",
                    StartDate = DateTime.Parse("2022-01-01"),
                    EndDate = DateTime.Parse("2022-12-31"),
                    Participants = 500,
                    Status = ClinicalTrialStatus.Ongoing,
                    TrialDuration = 365

                }
            );
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Already have data - not seeding");
        }
    }
}
