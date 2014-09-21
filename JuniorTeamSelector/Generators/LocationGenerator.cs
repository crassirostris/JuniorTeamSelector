namespace JuniorTeamSelector
{
    public class LocationGenerator
    {
        private readonly Auditory[] auditories;

        private int currentAuditory;
        private int currentComputerNumber;

        public LocationGenerator(Auditory[] auditories)
        {
            this.auditories = auditories;
        }

        public void Reset()
        {
            currentAuditory = 0;
            currentComputerNumber = 0;
        }

        public Location GetNext()
        {
            if (auditories[currentAuditory].Capacity <= currentComputerNumber)
            {
                ++currentAuditory;
                currentComputerNumber = 0;
            }
            return new Location(auditories[currentAuditory].Name, ++currentComputerNumber);
        }
    }
}