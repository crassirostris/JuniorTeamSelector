namespace JuniorTeamSelector
{
    public class TeamCredentials
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public int Round { get; set; }

        public TeamCredentials(string name, string login, int round)
        {
            Name = name;
            Login = login;
            Round = round;
        }

        public static TeamCredentials Parse(string line)
        {
            var chunks = line.Split(';');
            return new TeamCredentials(chunks[0], chunks[1], int.Parse(chunks[2]));
        }
    }
}