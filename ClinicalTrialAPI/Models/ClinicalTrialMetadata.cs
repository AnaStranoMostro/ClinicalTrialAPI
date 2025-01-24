using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using ClinicalTrialAPI.Helpers;
using Newtonsoft.Json;

namespace ClinicalTrialAPI.Models;

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

public enum ClinicalTrialStatus
{
    [EnumMember(Value = "Not Started")]
    NotStarted,
    [EnumMember(Value = "Ongoing")]
    Ongoing,
    [EnumMember(Value = "Completed")]
    Completed
}
