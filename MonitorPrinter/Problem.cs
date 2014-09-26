namespace MonitorPrinter
{
    public class Problem
    {
        public bool Solved { get; set; }

        public int Failed { get; set; }

        public int LastSubmitTime { get; set; }

        public int Penalty { get { return Solved ? LastSubmitTime + 20 * Failed : 0; } }
    }
}