using ProtoBuf;

namespace Core.DataStructures
{
    [ProtoContract]
    public class TeamCredentials
    {
        [ProtoMember(1, IsRequired = true)]
        public string Name { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Login { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public int Round { get; set; }

        public TeamCredentials(string name, string login, int round)
        {
            Name = name;
            Login = login;
            Round = round;
        }

        public TeamCredentials()
        {
        }

        public static TeamCredentials Parse(string line)
        {
            var chunks = line.Split(';');
            return new TeamCredentials(chunks[0], chunks[1], int.Parse(chunks[2]));
        }
    }
}