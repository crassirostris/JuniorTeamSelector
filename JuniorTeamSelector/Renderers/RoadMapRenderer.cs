﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnidecodeSharpFork;

namespace JuniorTeamSelector
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

        public void Render(string contestant, List<Team> teams)
        {
            var result = File.ReadAllText(mainTemplateFileName)
                .Replace("%CONTESTANT_NAME%", contestant);
            var roundTemplate = File.ReadAllText(roundTemplateFileName);
            var rounds = string.Empty;
            var involvedTeams = teams.Where(team => team.Contestants.Contains(contestant)).OrderBy(team => team.RoundNumber).ToList();
            foreach (var team in involvedTeams)
            {
                rounds += roundTemplate
                    .Replace("%ROUND_NUMBER%", team.RoundNumber.ToString(CultureInfo.InvariantCulture))
                    .Replace("%TEAM_NAME%", team.Name)
                    .Replace("%LOCATION_NAME%", team.Location.Name)
                    .Replace("%COMPUTER_NUMBER%", team.Location.ComputerNumber.ToString(CultureInfo.InvariantCulture));
            }
            result = result.Replace("%ROUNDS%", rounds);
            File.WriteAllText(Path.Combine(outputDirectory, string.Format("roadmap.{0}.html", contestant.Unidecode())), result);
        }
    }
}