using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.DataStructures;
using JuniorTeamSelector.Generators;
using Newtonsoft.Json;

namespace JuniorTeamSelector
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = JuniorTeamSelectorConfig.Instance;

            var contestants = ReadContestants(args);
            var teamCredentialses = ReadTeamsCredentials(args);
            var teams = new List<Team>();
            foreach (var file in new DirectoryInfo(config.OutputDirectory).GetFiles())
            {
                if (!(file.Name.StartsWith("team") && file.Name.EndsWith("html") && !file.Name.Contains("template")))
                    continue;
                var content = File.ReadAllText(file.FullName);
                teams.Add(ExtractTeam(teamCredentialses, content, contestants));
            }

            var missingContestants = new[]
            {
                contestants.First(c => c.Name == "Хашимов Ойбек"),
                contestants.First(c => c.Name == "Бахарев Артём")
            };
            var missingContestantIndex = 0;

            var vedernikov = contestants.First(c => c.Name == "Ведерников Максим");
            var borozdin = contestants.First(c => c.Name == "Бороздин Кирилл");

            foreach (var team in teams)
            {
                if (team.Contestants.Count != config.TeamMembersCount)
                    team.Contestants.Add(missingContestants[(missingContestantIndex = (missingContestantIndex + 1) % 2)]);

                var toAdd = new List<Contestant>();
                if (team.Contestants.Contains(vedernikov))
                {
                    team.Contestants.Remove(vedernikov);
                    toAdd.Add(borozdin);
                }
                if (team.Contestants.Contains(borozdin))
                {
                    team.Contestants.Remove(borozdin);
                    toAdd.Add(vedernikov);
                }
                foreach (var contestant in toAdd)
                    team.Contestants.Add(contestant);
            }

            File.WriteAllText(config.TeamsInfoFileName, JsonConvert.SerializeObject(new DataRepository(teams), Formatting.Indented));

            return;

            /*CheckArguments(args);

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
                Serializer.Serialize(fileStream, new DataRepository(teams));*/
        }


        static Regex roundNumberRegex = new Regex(@"Раунд (\d+)");
        static Regex teamNameRegex = new Regex(@"<div class=""team-name"">([-!.\w\s]+)</div>");
        static Regex teamLocationRegex = new Regex(@"<div class=""team-location"">Аудитория (\w+) Компьютер №(\d+)</div>");
        static Regex contestantRegex = new Regex(@"<td class=""contestant-name"">\s*(\w+\ \w+)\s*</td>");
        static Regex teamLoginRegex = new Regex(@"<div class=""team-location"">JudgeID (\w+)</div> ");

        private static Team ExtractTeam(TeamCredentials[] teamCredentialses, string content, Contestant[] totalContestants)
        {
            var roundNumber = int.Parse(roundNumberRegex.Match(content).Groups[1].Value);
            var locationMatch = teamLocationRegex.Match(content);
            var location = new Location(locationMatch.Groups[1].Value, int.Parse(locationMatch.Groups[2].Value));

            var teamName = teamNameRegex.Match(content).Groups[1].Value;

            var contestants =
                (from Match m in contestantRegex.Matches(content) select m.Groups[1].Value)
                .Select(e => totalContestants.First(c => c.Name == e))
                .ToList();

            return new Team(teamCredentialses.First(tc => tc.Name == teamName), contestants, location, roundNumber);
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
