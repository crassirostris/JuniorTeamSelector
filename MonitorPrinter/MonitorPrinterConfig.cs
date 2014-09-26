using System.IO;
using Newtonsoft.Json;

namespace MonitorPrinter
{
    public class MonitorPrinterConfig
    {
        private static string configFileName = "config.json";
        private static MonitorPrinterConfig instance;

        public string LogsDirectoryPath { get; set; }

        public string TeamsInfoFileName { get; set; }

        public int ProblemsCount { get; set; }

        public string OutputFile { get; set; }

        public static MonitorPrinterConfig Instance
        {
            get { return instance ?? (instance = LoadFrom(configFileName)); }
        }

        private static MonitorPrinterConfig LoadFrom(string path)
        {
            if (!File.Exists(path))
                return CreateDeafult();
            return JsonConvert.DeserializeObject<MonitorPrinterConfig>(File.ReadAllText(path));
        }

        private static MonitorPrinterConfig CreateDeafult()
        {
            var result = new MonitorPrinterConfig
            {
                TeamsInfoFileName = @"output\teams.dat",
                LogsDirectoryPath = @"logs",
                OutputFile = "monitor.txt",
                ProblemsCount = 24
            };
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(result, Formatting.Indented));
            return result;
        }
    }
}