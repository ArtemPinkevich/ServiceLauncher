namespace ServiceLauncher.Logic
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using Formatting = System.Xml.Formatting;

    class JsonHelper
    {
        //private static readonly Logger _logger = LogManager.GetLogger("JsonHelper");

        public static T SafeReadFromFile<T>(string fullName) where T : new()
        {
            if (File.Exists(fullName))
            {
                try
                {
                    var deserializeObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(fullName));
                    return deserializeObject == null ? new T() : deserializeObject;
                }
                catch (Exception e)
                {
                    //_logger.Error(e, $"Cannot read from file {fullName}! Message: {e.Message}");
#if DEBUG
                    throw;
#endif
                }
            }

            return new T();
        }

        public static void SafeSaveToFile<T>(string folderName, string fileNameWithExtension, T settingsObject)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string fullFileName = $"{folderName}\\{fileNameWithExtension}";

            if (settingsObject != null)
            {
                try
                {
                    File.WriteAllText(fullFileName, JsonConvert.SerializeObject(settingsObject, (Newtonsoft.Json.Formatting)Formatting.Indented));
                }
                catch (Exception e)
                {
                    //_logger.Error(e, $"Cannot write to file {fullFileName}! Message: {e.Message}");
#if DEBUG
                    throw;
#endif
                }
            }
        }
    }
}
