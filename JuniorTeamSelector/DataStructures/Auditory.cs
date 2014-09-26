namespace JuniorTeamSelector
{
    public class Auditory
    {
        public Auditory(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
        }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public static Auditory Parse(string line)
        {
            var chunks = line.Split(new[] { ' ', '\t' });
            return new Auditory(chunks[0], int.Parse(chunks[1]));
        }
    }
}