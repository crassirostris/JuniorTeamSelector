using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JuniorTeamSelector
{
    public class Program
    {
        private const string RoadMapMainTemplateFileName = @"templates\roadmap.template.html";
        private const string RoadMapRoundTemplateFileName = @"templates\roadmap.round.template.html";
        private const string TeamDescriptionMainTemplateFileName = @"templates\teamdescritption.template.html";
        private const string TeamDescriptionContestantTemplateFileName = @"templates\teamdescritption.contestant.template.html";
        private const string OutputDirectory = @"output\";
        private const int RoundsCount = 8;
        private const int TeamMembersCount = 3;

        static void Main(string[] args)
        {
            CheckArguments(args);

            CleanDirectory();

            var contestants = ReadContestants(args);
            var roundsGenerator = new RoundArrangementGenerator<Contestant>(contestants, TeamMembersCount);
            var teamNameGenerator = new TeamCredentialsGenerator(ReadTeamsCredentials(args));
            var locationGenerator = new LocationGenerator(ReadAuditories(args));

            var roadMapRenderer = new RoadMapRenderer(RoadMapMainTemplateFileName, RoadMapRoundTemplateFileName, OutputDirectory);
            var teamDescriptionRenderer = new TeamDescriptionRenderer(TeamDescriptionMainTemplateFileName, TeamDescriptionContestantTemplateFileName, OutputDirectory);

            var teams = GenerateTeams(roundsGenerator, locationGenerator, teamNameGenerator).ToList();

            foreach (var contestant in contestants)
                roadMapRenderer.Render(contestant, teams);
            foreach (var team in teams)
                teamDescriptionRenderer.Render(team);
        }

        private static void CleanDirectory()
        {
            foreach (var file in new DirectoryInfo(OutputDirectory).GetFiles().Where(file => file.Name.EndsWith(".html")))
                File.Delete(file.FullName);
        }

        private static IEnumerable<Team> GenerateTeams(RoundArrangementGenerator<Contestant> roundArrangementGenerator, LocationGenerator locationGenerator,
            TeamCredentialsGenerator teamCredentialsGenerator)
        {
            foreach (var round in roundArrangementGenerator.Generate(RoundsCount))
            {
                locationGenerator.Reset();
                foreach (var team in round.Arrangement.Select(members 
                    => new Team(teamCredentialsGenerator.GetNext(round.Number), members, locationGenerator.GetNext(), round.Number)))
                    yield return team;
            }
        }

        private static TeamCredentials[] ReadTeamsCredentials(string[] args)
        {
            return File.ReadAllLines(args[2])
                .Select(TeamCredentials.Parse)
                .ToArray();
        }

        private static Auditory[] ReadAuditories(string[] args)
        {
            return File.ReadAllLines(args[1])
                .Select(Auditory.Parse)
                .ToArray();
        }

        private static Contestant[] ReadContestants(string[] args)
        {
            return File.ReadAllLines(args[0])
                .Select(Contestant.Parse)
                .ToArray();
        }

        private static void CheckArguments(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            foreach (var filename in args)
                CheckFileExistance(filename);
        }

        private static void CheckFileExistance(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("Cannot find file {0}", filename);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: {0} <contestants_list_filename> <auditories_list> <team_names_list>", Environment.GetCommandLineArgs()[0]);
        }
    }
}
