using System.Collections.Generic;
using ProtoBuf;

namespace Core.DataStructures
{
    [ProtoContract]
    public class Team
    {
        [ProtoMember(1, IsRequired = true)]
        public TeamCredentials Credentials { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public List<Contestant> Contestants { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public Location Location { get; set; }

        [ProtoMember(4, IsRequired = true)]
        public int RoundNumber { get; set; }

        public Team(TeamCredentials credentials, List<Contestant> contestants, Location location, int roundNumber)
        {
            Credentials = credentials;
            Contestants = contestants;
            Location = location;
            RoundNumber = roundNumber;
        }

        public Team()
        {
        }

        #region Equality Members

        protected bool Equals(Team other)
        {
            return Equals(Credentials, other.Credentials) && Equals(Contestants, other.Contestants) && Equals(Location, other.Location) && RoundNumber == other.RoundNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Team) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Credentials != null ? Credentials.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Contestants != null ? Contestants.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RoundNumber;
                return hashCode;
            }
        }

        #endregion
    }
}