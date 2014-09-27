using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using Core.DataStructures;

namespace MonitorPrinter
{
    public class ContestantState : IComparable<ContestantState>
    {
        private readonly List<Submit> submits = new List<Submit>(); 

        public int SolvedTotal { get; private set; }

        public int Penalty { get { return Problems.Sum(p => p.Penalty); } }

        public Contestant Contestant { get; private set; }

        public int LastSubmit { get; private set; }

        public List<Problem> Problems { get; private set; }

        public ContestantState(Contestant contestant)
        {
            Contestant = contestant;
            var config = MonitorPrinterConfig.Instance;
            Problems = Enumerable.Range(0, config.ProblemsCount).Select(e => new Problem()).ToList();
        }

        public void Apply(Submit submit)
        {
            var problem = Problems[submit.ProblemNumber];
            if (!problem.Solved && !submit.CompilationError)
            {
                if (submit.Accepted)
                {
                    problem.Solved = true;
                    ++SolvedTotal;
                }
                else
                    ++problem.Failed;
                problem.LastSubmitTime = submit.Time;
            }
        }

        public int CompareTo(ContestantState other)
        {
            if (SolvedTotal != other.SolvedTotal) return other.SolvedTotal - SolvedTotal;
            if (Penalty != other.Penalty) return Penalty - other.Penalty;
            if (LastSubmit != other.LastSubmit) return other.LastSubmit - LastSubmit;
            return other.submits.Count - submits.Count;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Contestant.Name).Append('\t');
            sb.Append(SolvedTotal.ToString()).Append('\t');
            sb.Append(Penalty.ToString()).Append('\t');
            foreach (var problem in Problems)
            {
                if (problem.Solved)
                {
                    sb.Append('+');
                    if (problem.Failed > 0)
                        sb.Append(problem.Failed);
                    sb.Append(string.Format(" ({0}:{1:D2})", problem.LastSubmitTime / 60, problem.LastSubmitTime % 60));
                }
                else if (problem.Failed > 0)
                {
                    sb.Append('-').Append(problem.Failed);
                    sb.Append(string.Format(" ({0}:{1:D2})", problem.LastSubmitTime / 60, problem.LastSubmitTime % 60));
                }
                sb.Append('\t');
            }
            return sb.ToString();
        }
    }
}