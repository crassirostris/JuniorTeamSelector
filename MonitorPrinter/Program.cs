using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.DataStructures;
using Newtonsoft.Json;

namespace MonitorPrinter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = MonitorPrinterConfig.Instance;

            var dataRepository = JsonConvert
                .DeserializeObject<DataRepository>(File.ReadAllText(config.TeamsInfoFileName));

            var teamsMapping = dataRepository.Teams
                .ToDictionary(team => team.Credentials.Login.Substring(0, 6), team => team);

            var states = dataRepository.Teams
                .SelectMany(team => team.Contestants)
                .Distinct()
                .ToDictionary(c => c, c => new ContestantState(c));

            var submits = ReadAllSubmits(config).OrderBy(record => record.Time).ToList();
            foreach (var submit in submits)
            {
                if (!teamsMapping.ContainsKey(submit.Id))
                    continue;
                foreach (var contestant in teamsMapping[submit.Id].Contestants)
                    states[contestant].Apply(submit);
            }

            var rating = states.Values.OrderBy(e => e).ToList();

            File.WriteAllLines(config.OutputFile, rating.Select(e => e.ToString()).ToArray());
        }

        private static IEnumerable<Submit> ReadAllSubmits(MonitorPrinterConfig config)
        {
            if (!Directory.Exists(config.LogsDirectoryPath))
                return Enumerable.Empty<Submit>();
            return new DirectoryInfo(config.LogsDirectoryPath).GetFiles("*.log")
                .Select(fileInfo => File.ReadAllLines(fileInfo.FullName).Select(Submit.Parse))
                .SelectMany(e => e);
        }
    }
}
