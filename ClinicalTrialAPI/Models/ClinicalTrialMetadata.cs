using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ClinicalTrialAPI.Helpers;
using Newtonsoft.Json;

namespace ClinicalTrialAPI.Models;

/// <summary>
/// Metadata for a clinical trial.
/// </summary>
public class ClinicalTrialMetadata
{
    [Key]
    public required string TrialId { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Participants count must be greater than 0")]
    public int Participants { get; set; }

    [Required]
    [JsonConverter(typeof(JsonEnumHelper))]
    public ClinicalTrialStatus Status { get; set; }

    public int TrialDuration { get; set; }
}

/// <summary>
/// Status of a clinical trial.
/// </summary>
public enum ClinicalTrialStatus
{
    /// <summary>
    /// The trial has not started yet.
    /// </summary>
    [EnumMember(Value = "Not Started")]
    NotStarted,

    /// <summary>
    /// The trial is currently ongoing.
    /// </summary>
    [EnumMember(Value = "Ongoing")]
    Ongoing,

    /// <summary>
    /// The trial has been completed.
    /// </summary>
    [EnumMember(Value = "Completed")]
    Completed
}
