using System.Globalization;
using System.IO;
using System.Linq;
using Core.DataStructures;

namespace JuniorTeamSelector.Renderers
{
    public class TeamDescriptionRenderer
    {
        private readonly string mainTemplateFileName;
        private readonly string contestantTemplateFileName;
        private readonly string outputDirectory;

        public TeamDescriptionRenderer(string mainTemplateFileName, string contestantTemplateFileName, string outputDirectory)
        {
            this.mainTemplateFileName = mainTemplateFileName;
            this.contestantTemplateFileName = contestantTemplateFileName;
            this.outputDirectory = outputDirectory;
        }

        public void Render(Team team)
        {
            var result = File.ReadAllText(mainTemplateFileName)
                .Replace("%ROUND_NUMBER%", team.RoundNumber.ToString(CultureInfo.InvariantCulture))
                .Replace("%TEAM_NAME%", team.Credentials.Name)
                .Replace("%LOCATION_NAME%", team.Location.Name)
                .Replace("%COMPUTER_NUMBER%", team.Location.ComputerNumber.ToString(CultureInfo.InvariantCulture));
            var contestantTemplate = File.ReadAllText(contestantTemplateFileName);
            var contestants = team.Contestants.Aggregate(string.Empty, (current, t) => current + contestantTemplate.Replace("%CONTESTANT_NAME%", t.Name));
            result = result.Replace("%CONTESTANTS%", contestants);
            File.WriteAllText(Path.Combine(outputDirectory, string.Format("teamdescription.{0}.{1}.html", team.RoundNumber, team.Credentials.Name)), result);
        }
    }
}