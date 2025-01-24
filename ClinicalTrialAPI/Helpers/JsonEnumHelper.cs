
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ClinicalTrialAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ClinicalTrialAPI.Helpers;

public class JsonEnumHelper : StringEnumConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        ClinicalTrialStatus enumValue = (ClinicalTrialStatus)value;
        string stringValue = enumValue.GetEnumDisplayText(); // Get the display name of the enum value
        writer.WriteValue(stringValue);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            string enumString = (string)reader.Value;
            foreach (object? value in Enum.GetValues(objectType))
            {
                ClinicalTrialStatus enumValue = (ClinicalTrialStatus)value;
                if (enumValue.GetEnumDisplayText() == enumString)
                {
                    return value;
                }
            }
        }
        return base.ReadJson(reader, objectType, existingValue, serializer);
    }
}

public static class EnumExtensions
{
    public static string GetEnumDisplayText(this Enum value)
    {
        DisplayAttribute? displayAttribute = value.GetType()
            .GetMember(value.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? value.ToString();
    }
}
