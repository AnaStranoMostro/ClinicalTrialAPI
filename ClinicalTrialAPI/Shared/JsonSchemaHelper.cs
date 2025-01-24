using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ClinicalTrialAPI.Helpers
{
    public static class JsonSchemaHelper
    {
        private static JSchema GetSchemaFromResource(this string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException("Schema resource not found.");
            }
            using StreamReader reader = new StreamReader(stream);
            string schemaText = reader.ReadToEnd();
            return JSchema.Parse(schemaText);
        }

        public static bool ValidateJson(this string jsonContent, out IList<string> validationErrors)
        {
            //get the schema from Resources
             Assembly.GetExecutingAssembly();
            string resourceName = "ClinicalTrialAPI.Properties.Resources.jsonschema.json";

            JSchema schema = JsonSchemaHelper.GetSchemaFromResource(resourceName);

            JObject jsonObject = JObject.Parse(jsonContent);
            return jsonObject.IsValid(schema, out validationErrors);
        }


        public static JObject ParseJson(this string jsonContent)
        {
            return JObject.Parse(jsonContent);
        }
    }
}
