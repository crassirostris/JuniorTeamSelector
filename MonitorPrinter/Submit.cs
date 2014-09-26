using System.Text.RegularExpressions;

namespace MonitorPrinter
{
    public class Submit
    {
        private static readonly Regex parseRegex = new Regex(@"(\w+)\ +(\w+)\ +(\d+)\ +(\d+)\ (.*)");

        public string Id { get; private set; }

        public int ProblemNumber { get; private set; }

        public int Time { get; private set; }

        public int TestNumber { get; private set; }

        public string Verdict { get; private set; }

        public bool CompilationError { get { return Verdict == "Compilation error"; } }

        public bool Accepted { get { return Verdict == "Accepted"; } }

        public Submit(string id, int problemNumber, int time, int testNumber, string verdict)
        {
            Id = id;
            ProblemNumber = problemNumber;
            Time = time;
            TestNumber = testNumber;
            Verdict = verdict;
        }

        public static Submit Parse(string line)
        {
            var m = parseRegex.Match(line);
            return new Submit(m.Groups[0].Value, m.Groups[1].Value[0] - 'A', int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), m.Groups[4].Value);
        }
    }
}