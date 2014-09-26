using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.DataStructures;
using JuniorTeamSelector.Generators;
using JuniorTeamSelector.Renderers;
using ProtoBuf;

namespace JuniorTeamSelector
{
    public class Program
    {
        static void Main(string[] args)
        {
            CheckArguments(args);

            var config = JuniorTeamSelectorConfig.Instance;

            CleanDirectory(config);

            var contestants = ReadContestants(args);
            var roundsGenerator = new RoundArrangementGenerator<Contestant>(contestants, config.TeamMembersCount);
            var teamNameGenerator = new TeamCredentialsGenerator(ReadTeamsCredentials(args));
            var locationGenerator = new LocationGenerator(ReadAuditories(args));

            var roadMapRenderer = new RoadMapRenderer(config.RoadMapMainTemplateFileName, config.RoadMapRoundTemplateFileName, config.OutputDirectory);
            var teamDescriptionRenderer = new TeamDescriptionRenderer(config.TeamDescriptionMainTemplateFileName, config.TeamDescriptionContestantTemplateFileName, config.OutputDirectory);
            var auditoriesArrangementRenderer = new AuditoriesArrangementRenderer(config.AuditoriesArrangementFileName);

            var teams = GenerateTeams(config, roundsGenerator, locationGenerator, teamNameGenerator).ToList();

            foreach (var contestant in contestants)
                roadMapRenderer.Render(contestant, teams);
            foreach (var team in teams)
                teamDescriptionRenderer.Render(team);

            auditoriesArrangementRenderer.Render(teams);

            using (var fileStream = File.Open(config.TeamsInfoFileName, FileMode.OpenOrCreate))
                Serializer.Serialize(fileStream, new DataRepository(teams));
        }

        private static void CleanDirectory(JuniorTeamSelectorConfig config)
        {
            foreach (var file in new DirectoryInfo(config.OutputDirectory).GetFiles().Where(file => file.Name.EndsWith(".html")))
                File.Delete(file.FullName);
        }

        private static IEnumerable<Team> GenerateTeams(JuniorTeamSelectorConfig config, RoundArrangementGenerator<Contestant> roundArrangementGenerator, LocationGenerator locationGenerator, TeamCredentialsGenerator teamCredentialsGenerator)
        {
            foreach (var round in roundArrangementGenerator.Generate(config.RoundsCount))
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
