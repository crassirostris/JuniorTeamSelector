namespace JuniorTeamSelector
{
    public class Contestant
    {
        public string Name { get; private set; }

        public string CasedName { get; private set; }

        public Contestant(string name, string casedName)
        {
            Name = name;
            CasedName = casedName;
        }

        public static Contestant Parse(string line)
        {
            var chunks = line.Split(';');
            return new Contestant(chunks[0], chunks[1]);
        }

        #region Equality Members

        protected bool Equals(Contestant other)
        {
            return string.Equals(Name, other.Name) && string.Equals(CasedName, other.CasedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Contestant) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (CasedName != null ? CasedName.GetHashCode() : 0);
            }
        }

        #endregion
    }
}