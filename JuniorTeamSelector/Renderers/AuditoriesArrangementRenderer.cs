using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.DataStructures;

namespace JuniorTeamSelector.Renderers
{
    public class AuditoriesArrangementRenderer
    {
        private readonly string auditoriesArrangementFileName;

        public AuditoriesArrangementRenderer(string auditoriesArrangementFileName)
        {
            this.auditoriesArrangementFileName = auditoriesArrangementFileName;
        }

        public void Render(List<Team> teams)
        {
            File.WriteAllLines(auditoriesArrangementFileName, teams
                .Select(team => string.Format("{0} {1}-{2}", team.Credentials.Login.Substring(0, 6), team.Location.Name, team.Location.ComputerNumber))
                .ToArray());
        }
    }
}