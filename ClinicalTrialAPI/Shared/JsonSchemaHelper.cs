using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;

namespace ClinicalTrialAPI.Helpers
{
    public static class JsonSchemaHelper
    {
        private static JSchema GetSchemaFromResource(this string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Schema resource not found.");
                }
                using (var reader = new StreamReader(stream))
                {
                    var schemaText = reader.ReadToEnd();
                    return JSchema.Parse(schemaText);
                }
            }
        }

        public static bool ValidateJson(this string jsonContent, out IList<string> validationErrors)
        {
            //get the schema from Resources
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ClinicalTrialAPI.Properties.Resources.jsonschema.json";
            string schemaText;

            JSchema schema = JsonSchemaHelper.GetSchemaFromResource(resourceName);

            var jsonObject = JObject.Parse(jsonContent);
            return jsonObject.IsValid(schema, out validationErrors);        
        }


        public static JObject ParseJson(this string jsonContent)
        {
            return JObject.Parse(jsonContent);
        }
    }
}
