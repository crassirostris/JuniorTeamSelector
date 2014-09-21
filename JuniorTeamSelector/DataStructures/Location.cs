namespace JuniorTeamSelector
{
    public class Location
    {
        public Location(string name, int computerNumber)
        {
            Name = name;
            ComputerNumber = computerNumber;
        }

        public string Name { get; set; }

        public int ComputerNumber { get; set; }
    }
}