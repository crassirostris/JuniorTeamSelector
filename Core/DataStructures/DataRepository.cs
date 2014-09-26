using System.Collections.Generic;
using ProtoBuf;

namespace Core.DataStructures
{
    [ProtoContract]
    public class DataRepository
    {
        [ProtoMember(1, IsRequired = true)]
        public List<Team> Teams { get; private set; }

        public DataRepository()
            : this(new List<Team>())
        {
        }

        public DataRepository(List<Team> teams)
        {
            Teams = teams;
        }
    }
}
