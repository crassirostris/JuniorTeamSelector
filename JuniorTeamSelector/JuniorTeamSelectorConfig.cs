using System.IO;
using Newtonsoft.Json;

namespace JuniorTeamSelector
{
    public class JuniorTeamSelectorConfig
    {
        private static string configFileName = "config.json";
        private static JuniorTeamSelectorConfig instance;

        public string TeamsInfoFileName { get; set; }

        public string RoadMapMainTemplateFileName { get; set; }

        public string RoadMapRoundTemplateFileName { get; set; }

        public string OutputDirectory { get; set; }

        public string TeamDescriptionMainTemplateFileName { get; set; }

        public string TeamDescriptionContestantTemplateFileName { get; set; }

        public string AuditoriesArrangementFileName { get; set; }
        public int TeamMembersCount { get; set; }

        public int RoundsCount { get; set; }

        public static JuniorTeamSelectorConfig Instance
        {
            get { return instance ?? (instance = LoadFrom(configFileName)); }
        }

        private static JuniorTeamSelectorConfig LoadFrom(string path)
        {
            if (!File.Exists(path))
                return CreateDeafult();
            return JsonConvert.DeserializeObject<JuniorTeamSelectorConfig>(File.ReadAllText(path));
        }

        private static JuniorTeamSelectorConfig CreateDeafult()
        {
            var result = new JuniorTeamSelectorConfig
            {
                TeamsInfoFileName = @"output\teams.dat",
                AuditoriesArrangementFileName = @"output\auditories.arrangement.txt",
                RoadMapMainTemplateFileName = @"templates\roadmap.template.html",
                RoadMapRoundTemplateFileName = @"templates\roadmap.round.template.html",
                TeamDescriptionMainTemplateFileName = @"templates\teamdescritption.template.html",
                TeamDescriptionContestantTemplateFileName = @"templates\teamdescritption.contestant.template.html",
                OutputDirectory = @"output\",
                RoundsCount = 8,
                TeamMembersCount = 3
            };
            File.WriteAllText(configFileName, JsonConvert.SerializeObject(result));
            return result;
        }
    }
}