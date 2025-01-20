
using System;
using System.ComponentModel.DataAnnotations;
using ClinicalTrialAPI.Extensions;
using Newtonsoft.Json;

namespace ClinicalTrialAPI.Models;

public class ClinicalTrialMetadata
{
    [Key]
    public string TrialId { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Participants count must be greater than 0")]
    public int Participants { get; set; }

    [Required]
    [JsonConverter(typeof(JsonEnumStringConverter))]
    public ClinicalTrialStatus Status { get; set; }
}

public enum ClinicalTrialStatus
{
    [Display(Name = "Not Started")]
    NotStarted,
    Ongoing,
    Completed
}