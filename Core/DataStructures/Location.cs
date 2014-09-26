using ProtoBuf;

namespace Core.DataStructures
{
    [ProtoContract]
    public class Location
    {
        [ProtoMember(1, IsRequired = true)]
        public string Name { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public int ComputerNumber { get; set; }

        public Location(string name, int computerNumber)
        {
            Name = name;
            ComputerNumber = computerNumber;
        }

        public Location()
        {
        }
    }
}