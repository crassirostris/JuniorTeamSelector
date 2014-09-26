using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.DataStructures;
using UnidecodeSharpFork;

namespace JuniorTeamSelector.Renderers
{
    public class RoadMapRenderer
    {
        private readonly string mainTemplateFileName;
        private readonly string roundTemplateFileName;
        private readonly string outputDirectory;

        public RoadMapRenderer(string mainTemplateFileName, string roundTemplateFileName, string outputDirectory)
        {
            this.mainTemplateFileName = mainTemplateFileName;
            this.roundTemplateFileName = roundTemplateFileName;
            this.outputDirectory = outputDirectory;
        }

        public void Render(Contestant contestant, List<Team> teams)
        {
            var result = File.ReadAllText(mainTemplateFileName)
                .Replace("%CONTESTANT_NAME%", contestant.CasedName);
            var roundTemplate = File.ReadAllText(roundTemplateFileName);
            var rounds = string.Empty;
            var involvedTeams = teams.Where(team => team.Contestants.Contains(contestant)).OrderBy(team => team.RoundNumber).ToList();
            foreach (var team in involvedTeams)
            {
                rounds += roundTemplate
                    .Replace("%ROUND_NUMBER%", team.RoundNumber.ToString(CultureInfo.InvariantCulture))
                    .Replace("%TEAM_NAME%", team.Credentials.Name)
                    .Replace("%LOCATION_NAME%", team.Location.Name)
                    .Replace("%COMPUTER_NUMBER%", team.Location.ComputerNumber.ToString(CultureInfo.InvariantCulture))
                    .Replace("%TIMUS_ID%", team.Credentials.Login);
            }
            result = result.Replace("%ROUNDS%", rounds);
            File.WriteAllText(Path.Combine(outputDirectory, string.Format("roadmap.{0}.html", contestant.Name.Unidecode())), result);
        }
    }
}