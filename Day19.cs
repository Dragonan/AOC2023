namespace AOC2023
{
    public class Workflow
    {
        public Workflow(string name, WorkflowRule[] rules, string finalResult)
        {
            Name = name;
            Rules = rules;
            FinalResult = finalResult;
        }

        public string Name { get; }
        public WorkflowRule[] Rules { get; }
        public string FinalResult { get; }

        public string Check(Gear gear)
        {
            foreach (var rule in Rules)
            {
                var result = rule.Check(gear);
                if (result != string.Empty)
                    return result;
            }
            return FinalResult;
        }

        public GearAttributeRanges GetRangesForResult(int ruleToSucceed)
        {
            var ranges = new List<AttributeRange>();
            for (int i = 0; i < Rules.Length && i < ruleToSucceed; i++)
            {
                var range = Rules[i].GetAcceptedRange();
                range.Invert();
                ranges.Add(range);
            }
            if (ruleToSucceed < Rules.Length)
                ranges.Add(Rules[ruleToSucceed].GetAcceptedRange());
            return new GearAttributeRanges(ranges);
        }
    }

    public class WorkflowRule
    {
        private readonly char attribute;
        private readonly int expectedComparison;
        private readonly int value;

        public WorkflowRule(char attribute, int expectedComparison, int value, string result)
        {
            this.attribute = attribute;
            this.expectedComparison = expectedComparison;
            this.value = value;
            Result = result;
        }

        public string Result { get; }

        public string Check(Gear gear)
        {
            if (gear.Attributes[attribute].CompareTo(value) == expectedComparison)
                return Result;
            return string.Empty;
        }

        public AttributeRange GetAcceptedRange()
        {
            var start = 1;
            var end = 4000;

            switch (expectedComparison)
            {
                case -1: end = value - 1; break;
                case 1: start = value + 1; break;
                default: break;
            }

            return new (attribute, new (start, end));
        }
    }

    public class Gear
    {
        public Gear(int x, int m, int a, int s)
        {
            Attributes = new Dictionary<char, int>
                { {'x',x},{'m',m},{'a',a},{'s',s} };
        }

        public Gear(IEnumerable<int> values)
        {
            Attributes = new Dictionary<char, int>
            { 
                {'x',values.ElementAt(0)},
                {'m',values.ElementAt(1)},
                {'a',values.ElementAt(2)},
                {'s',values.ElementAt(3)}
            };
        }

        public Dictionary<char,int> Attributes { get; }

        public int X { get => Attributes['x']; }
        public int M { get => Attributes['m']; }
        public int A { get => Attributes['a']; }
        public int S { get => Attributes['s']; }
    }

    public class AttributeRange
    {
        public AttributeRange(char attribute, IntRange range)
        {
            Attribute = attribute;
            Range = range;
        }

        public char Attribute { get; }
        public IntRange Range { get; private set; }

        public void Invert()
        {
            if (Range.Start == 1)
                Range = new IntRange(Range.End + 1, 4000);
            else if (Range.End == 4000)
                Range = new IntRange(1, Range.Start - 1);
        }
    }

    public class GearAttributeRanges
    {
        public GearAttributeRanges()
        {
            AttributeRanges = new Dictionary<char, IntRange>
            {
                {'x', new (1,4000)},
                {'m', new (1,4000)},
                {'a', new (1,4000)},
                {'s', new (1,4000)}
            };
        }

        public GearAttributeRanges(List<AttributeRange> ranges)
            : this()
        {
            var groups = ranges.GroupBy(r => r.Attribute);
            foreach (var group in groups)
            {
                var attrRanges = group.Select(g => g.Range);
                AttributeRanges[group.Key] = new IntRange(
                                                attrRanges.Max(r => r.Start),
                                                attrRanges.Min(r => r.End)
                                            );
            }
        }

        public GearAttributeRanges(GearAttributeRanges original)
        {
            AttributeRanges = new Dictionary<char, IntRange>
            {
                {'x', original.AttributeRanges['x']},
                {'m', original.AttributeRanges['m']},
                {'a', original.AttributeRanges['a']},
                {'s', original.AttributeRanges['s']}
            };
        }

        public Dictionary<char, IntRange> AttributeRanges { get; }

        public IntRange X => AttributeRanges['x'];
        public IntRange M => AttributeRanges['m'];
        public IntRange A => AttributeRanges['a'];
        public IntRange S => AttributeRanges['s'];

        public GearAttributeRanges Combine(GearAttributeRanges otherRanges)
        {
            var copy = new GearAttributeRanges(this);
            foreach (var key in otherRanges.AttributeRanges.Keys)
            {
                var myRange = copy.AttributeRanges[key];
                var otherRange= otherRanges.AttributeRanges[key];
                copy.AttributeRanges[key] = new IntRange(
                                                Math.Max(myRange.Start,otherRange.Start), 
                                                Math.Min(myRange.End,otherRange.End)
                                            );
            }
            return copy;
        }
    }

    public struct IntRange
    {
        public IntRange(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
        }

        public int Start { get; set; }
        public int End { get; set; }

        public override string ToString()
        {
            return Start + " - " + End;
        }
    }

    public static class Day19
    {
        static Dictionary<string, Workflow> Workflows;
        static Dictionary<string, List<GearAttributeRanges>> RangesToReachWorkflow; 


        public static void Solve()
        {
            var data = input.RemoveRF().Split("\n\n").Select(r => r.Split('\n').ToArray()).ToArray();
            Workflows = ParseWorkflows(data[0]).ToDictionary(k => k.Name, v => v);
            var gears = ParseGears(data[1]);

            SolvePart2();
        }

        static void SolvePart1(List<Gear> gears)
        {
            var sum = 0;
            foreach (var gear in gears)
            {
                string nextWorkflow = "in";
                while (nextWorkflow != "R" && nextWorkflow != "A" && nextWorkflow != string.Empty)
                {
                    nextWorkflow = Workflows[nextWorkflow].Check(gear);
                }

                if (nextWorkflow == "A")
                    sum += gear.Attributes.Values.Sum();
            }

            Console.WriteLine(sum);
        }

        static void SolvePart2()
        {
            RangesToReachWorkflow = new Dictionary<string, List<GearAttributeRanges>>();
            var gearAttributes = FindRangeToReachWorkflow();

            var sum = 0L;

            for (int x = 1; x <= 4000; x++)
            {
                var rangesThatMatchX = gearAttributes.Where(g => g.X.Start <= x && x <= g.X.End).ToArray();
                var nextRanges = gearAttributes.Select(r => r.X.Start).Where(r => x < r);
                if (!rangesThatMatchX.Any())
                {
                    if (!nextRanges.Any())
                        break;
                    x = nextRanges.Min() - 1;
                    continue;
                }

                var nextEnd = rangesThatMatchX.Min(g => g.X.End);
                if (nextRanges.Any())
                    nextEnd = Math.Min(nextEnd,nextRanges.Min() - 1);
                var toSkipX = nextEnd - x + 1;
                x += toSkipX - 1;

                for (int m = 1; m <= 4000; m++)
                {
                    var rangesThatMatchM = rangesThatMatchX.Where(g => g.M.Start <= m && m <= g.M.End).ToArray();
                    nextRanges = rangesThatMatchX.Select(r => r.M.Start).Where(r => m < r);
                    if (!rangesThatMatchM.Any())
                    {
                        if (!nextRanges.Any())
                            break;
                        m = nextRanges.Min() - 1;
                        continue;
                    }
                    
                    nextEnd = rangesThatMatchM.Min(g => g.M.End);
                    if (nextRanges.Any())
                        nextEnd = Math.Min(nextEnd,nextRanges.Min() - 1);
                    var toSkipM = nextEnd - m + 1;
                    m += toSkipM - 1;

                    for (int a = 1; a <= 4000; a++)
                    {
                        var rangesThatMatchA = rangesThatMatchM.Where(g => g.A.Start <= a && a <= g.A.End).ToArray();
                        nextRanges = rangesThatMatchM.Select(r => r.A.Start).Where(r => a < r);
                        if (!rangesThatMatchA.Any())
                        {
                            if (!nextRanges.Any())
                                break;
                            a = nextRanges.Min() - 1;
                            continue;
                        }
                        
                        nextEnd = rangesThatMatchA.Min(g => g.A.End);
                        if (nextRanges.Any())
                            nextEnd = Math.Min(nextEnd,nextRanges.Min() - 1);
                        var toSkipA = nextEnd - a + 1;
                        a += toSkipA - 1;

                        for (int s = 1; s <= 4000; s++)
                        {
                            var rangesThatMatchS = rangesThatMatchA.Where(g => g.S.Start <= s && s <= g.S.End).ToArray();
                            nextRanges = rangesThatMatchA.Select(r => r.S.Start).Where(r => s < r);
                            if (!rangesThatMatchS.Any())
                            {
                                if (!nextRanges.Any())
                                    break;
                                s = nextRanges.Min() - 1;
                                continue;
                            }
                            
                            nextEnd = rangesThatMatchS.Min(g => g.S.End);
                            if (nextRanges.Any())
                                nextEnd = Math.Min(nextEnd,nextRanges.Min() - 1);
                            var toSkipS = nextEnd - s + 1;
                            s += toSkipS - 1;
                            sum += (long)toSkipS * toSkipA * toSkipM * toSkipX;
                        }
                    }
                }
            }

            Console.WriteLine(sum);
        }

        static List<GearAttributeRanges> FindRangeToReachWorkflow(string lookingFor = "A")
        {
            var result = new List<GearAttributeRanges>();
            var finalWorkflows = Workflows.Values.Where(w => w.Rules.Any(r => r.Result == lookingFor) || w.FinalResult == lookingFor);
            foreach (var workflow in finalWorkflows)
            {
                var workflowResult = new List<GearAttributeRanges>();
                for (int i = 0; i < workflow.Rules.Length; i++)
                {
                    if (workflow.Rules[i].Result != lookingFor)
                        continue;

                    workflowResult.Add(workflow.GetRangesForResult(i));
                    
                }

                if (workflow.FinalResult == lookingFor)
                    workflowResult.Add(workflow.GetRangesForResult(workflow.Rules.Length));

                if (workflow.Name == "in")
                {
                    result.AddRange(workflowResult);
                    continue;
                }

                if (!RangesToReachWorkflow.ContainsKey(workflow.Name))
                    RangesToReachWorkflow.Add(workflow.Name, FindRangeToReachWorkflow(workflow.Name));
                var paths = RangesToReachWorkflow[workflow.Name];
                foreach (var path in paths)
                    foreach(var wr in workflowResult)
                        result.Add(wr.Combine(path));

            }
            return result;
        }

        static List<Workflow> ParseWorkflows(string[] data)
        {
            var workflows = new List<Workflow>();
            foreach (var row in data)
            {
                var separator = row.IndexOf('{');
                var name = row.Substring(0,separator);
                var rulesString = row.Substring(separator+1,row.Length - 2 - separator).Split(',').ToArray();
                string finalResult = rulesString.Last();
                rulesString = rulesString.SkipLast(1).ToArray();
                var rules = new WorkflowRule[rulesString.Length];
                var i = 0;
                foreach (var rule in rulesString)
                {
                    var attr = rule[0];
                    var comparer = rule[1];
                    var expectedComparison = comparer == '>' ? 1 : comparer == '<' ? -1 : 0;
                    separator = rule.IndexOf(':');
                    var value = int.Parse(rule.Substring(2, separator - 2));
                    var result = rule.Substring(separator+1);
                    rules[i] = new (attr, expectedComparison, value, result);
                    i++;
                }
                workflows.Add(new (name, rules, finalResult));
            }
            return workflows;
        }

        static List<Gear> ParseGears(string[] data)
        {
            return data.Select(g => new Gear(g.Substring(1,g.Length-2).Split(',').Select(a => int.Parse(a.Substring(2))))).ToList();
        }

        static string input = @""; //paste it manually from the page
    }
}