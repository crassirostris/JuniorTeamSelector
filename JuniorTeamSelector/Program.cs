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

            var contestants = ReadContestantsNames(args);
            var roundsGenerator = new RoundArrangementGenerator(contestants, TeamMembersCount);
            var teamNameGenerator = new TeamNameGenerator(ReadTeamNames(args));
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

        private static IEnumerable<Team> GenerateTeams(RoundArrangementGenerator roundArrangementGenerator, LocationGenerator locationGenerator,
            TeamNameGenerator teamNameGenerator)
        {
            foreach (var round in roundArrangementGenerator.Generate(RoundsCount))
            {
                locationGenerator.Reset();
                foreach (var team in round.Arrangement.Select(members 
                    => new Team(teamNameGenerator.GetNext(), members, locationGenerator.GetNext(), round.Number)))
                    yield return team;
            }
        }

        private static string[] ReadTeamNames(string[] args)
        {
            return File.ReadAllLines(args[2]);
        }

        private static Auditory[] ReadAuditories(string[] args)
        {
            return File.ReadAllLines(args[1])
                .Select(line =>
                {
                    var chunks = line.Split(new[] { ' ', '\t' });
                    return new Auditory(chunks[0], int.Parse(chunks[1]));
                })
                .ToArray();
        }

        private static string[] ReadContestantsNames(string[] args)
        {
            return File.ReadAllLines(args[0]);
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
