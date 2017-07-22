using System;

namespace InSoundzTest
{
    public static class ConfigurationManager
    {
        private static Configuration Instance { get; set; }

        public static Configuration GetConfiguration()
        {
            if (Instance != null)
            {
                return Instance;
            }

            Instance = new Configuration
            {
                Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]),
                OutputPath = System.Configuration.ConfigurationManager.AppSettings["OutputPath"],
                FfmpegPath = System.Configuration.ConfigurationManager.AppSettings["FFmpegPath"],
                ResourcesDirectory = System.Configuration.ConfigurationManager.AppSettings["ResourcesDirectory"],
            };

            return Instance;
        }
    }
}
