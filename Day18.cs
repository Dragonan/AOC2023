using System.Diagnostics;
using System.Drawing;

namespace AOC2023
{
    public class Line
    {
        public Line(Point start, Point end)
        {
            Start = new Point(Math.Min(start.X, end.X),Math.Min(start.Y,end.Y));
            End = new Point(Math.Max(start.X, end.X),Math.Max(start.Y,end.Y));
        }

        public bool IsHorizontal { get => Start.Y == End.Y; }

        public Point Start { get; }
        public Point End { get; }

        public override string ToString()
        {
            return $"({Start.X},{Start.Y})-({End.X},{End.Y})";
        }
    }

    public class NumRange
    {
        public NumRange(long start, long end)
        {
            Start = start;
            End = end;
        }

        public long Start { get; set; }
        public long End { get; set; }

        public override string ToString()
        {
            return $"{Start} - {End}";
        }
    }

    public static class Day18
    {
        public static void Solve()
        {
            var commands = input.Split('\n').Select(c => c.TrimEnd().Split(' ').ToArray()).ToArray();
            var lines = ParseCommandsForPart2(commands);
            var area = CalculateArea(lines);
            Console.WriteLine(area);
        }

        static List<Line> ParseCommandsForPart1(string[][] commands)
        {
            var lines = new List<Line>();
            var lastPoint = new Point(0, 0);
            foreach (var command in commands)
            {
                var dir = command[0];
                var distance = int.Parse(command[1]);
                var nextPoint = lastPoint;

                switch (dir)
                {
                    case "U": nextPoint.Y -= distance; break;
                    case "D": nextPoint.Y += distance; break;
                    case "L": nextPoint.X -= distance; break;
                    case "R": nextPoint.X += distance; break;
                    default: break;
                }

                lines.Add(new (lastPoint, nextPoint));
                lastPoint = nextPoint;
            }

            return lines;
        }

        static List<Line> ParseCommandsForPart2(string[][] commands)
        {
            var lines = new List<Line>();
            var lastPoint = new Point(0, 0);
            foreach (var command in commands)
            {
                var distance = int.Parse(command[2].Substring(2,5), System.Globalization.NumberStyles.HexNumber);
                var dir = command[2][7];
                var nextPoint = lastPoint;
                
                switch (dir)
                {
                    case '3': nextPoint.Y -= distance; break;
                    case '1': nextPoint.Y += distance; break;
                    case '2': nextPoint.X -= distance; break;
                    case '0': nextPoint.X += distance; break;
                    default: break;
                }

                lines.Add(new (lastPoint, nextPoint));
                lastPoint = nextPoint;
            }

            return lines;
        }

        static long CalculateArea(List<Line> lines)
        {
            var orderedLines = lines.GroupBy(l => l.Start.Y).OrderBy(pg => pg.Key).ToList();
            var area = 0L;
            var ranges = new List<NumRange>();
            for (int i = 0; i < orderedLines.Count; i++)
            {
                long fill = ranges.Sum(r => r.End - r.Start + 1);
                var horizontal = orderedLines[i].Where(l => l.IsHorizontal).OrderBy(l => l.Start.X).ToList();
                
                for (int j = 0; j < horizontal.Count; j++)
                {
                    var lineStart = horizontal[j].Start.X;
                    var lineEnd = horizontal[j].End.X;
                    
                    // close completed ranges
                    var ri = ranges.FindIndex(r => r.Start == lineStart && r.End == lineEnd);
                    if (ri >= 0)
                    {
                        ranges.RemoveAt(ri);
                        continue;
                    }

                    // extend/shorten ranges
                    var range = ranges.FirstOrDefault(r => r.Start == lineStart || r.Start == lineEnd);
                    if (range != null)
                    {
                        if (lineStart < range.Start)
                            fill += range.Start - lineStart;
                        range.Start = range.Start == lineStart ? lineEnd : lineStart;
                        continue;
                    }
                    range = ranges.FirstOrDefault(r => r.End == lineStart || r.End == lineEnd);
                    if (range != null)
                    {
                        if (range.End < lineEnd)
                            fill += lineEnd - range.End;
                        range.End = range.End == lineStart ? lineEnd : lineStart;
                        continue;
                    }

                    //split ranges
                    range = ranges.FirstOrDefault(r => r.Start < lineStart && lineEnd < r.End);
                    if (range != null)
                    {
                        ranges.Remove(range);
                        ranges.Add(new (range.Start, lineStart));
                        ranges.Add(new (lineEnd,range.End));
                        continue;
                    }

                    //add new range
                    ranges.Add(new (lineStart, lineEnd));
                    fill += lineEnd - lineStart + 1;
                    
                }

                // merge ranges
                for (int k = 0; k < ranges.Count - 1; k++)
                {
                    var range = ranges[k];
                    var extension = ranges.Skip(k+1).FirstOrDefault(r => range.Start == r.End || range.End == r.Start);
                    if (extension != default)
                    {
                        ranges[k] = new (Math.Min(range.Start, extension.Start), Math.Max(range.End, extension.End));
                        ranges.Remove(extension);
                        fill--;
                        k--;
                    }
                }

                area += fill;

                //fill empty spaces until next horizontal line
                if (i < orderedLines.Count - 1)
                    area += ranges.Sum(r => r.End - r.Start + 1) * (orderedLines[i+1].Key - orderedLines[i].Key - 1);
            }

            return area;
        }

        static string test = @"";

        static string input = @""; //paste it manually from the page
    }
}