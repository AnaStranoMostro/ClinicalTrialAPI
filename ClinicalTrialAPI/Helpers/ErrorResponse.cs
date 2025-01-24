namespace ClinicalTrialAPI.Helpers;

/// <summary>
/// Represents an error response containing a list of error messages.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the list of error messages.
    /// </summary>
    public IList<string> Errors { get; set; }
}
