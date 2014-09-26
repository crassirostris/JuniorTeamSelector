using ProtoBuf;

namespace Core.DataStructures
{
    [ProtoContract]
    public class Auditory
    {
        [ProtoMember(1, IsRequired = true)]
        public string Name { get; private set; }

        [ProtoMember(2, IsRequired = true)]
        public int Capacity { get; private set; }

        public Auditory(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
        }

        public static Auditory Parse(string line)
        {
            var chunks = line.Split(new[] { ' ', '\t' });
            return new Auditory(chunks[0], int.Parse(chunks[1]));
        }
    }
}