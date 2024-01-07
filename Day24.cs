using System.Numerics;

namespace AOC2023
{
    public struct PointL
    {
        public PointL(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long X { get; }
        public long Y { get; }
    }

    public class Hailstone
    {
        public Hailstone(Vector<long> coords, Vector<long> velocity)
        {
            Coords = coords;
            Velocity = velocity;
            EndCoords = Vector<long>.Zero;
            StartCoords = Vector<long>.Zero;
        }

        public Vector<long> Coords { get; }
        public Vector<long> Velocity { get; }
        public long X => Coords[0];
        public long Y => Coords[1];
        public long Z => Coords[2];
        public long XChange => Velocity[0];
        public long YChange => Velocity[1];
        public long ZChange => Velocity[2];
        public Vector<long> StartCoords { get; set; }
        public long XStart => StartCoords[0];
        public long YStart => StartCoords[1];
        public long ZStart => StartCoords[2];
        public Vector<long> EndCoords { get; set; }
        public long XEnd => EndCoords[0];
        public long YEnd => EndCoords[1];
        public long ZEnd => EndCoords[2];
        public bool SegmentCalculated { get; set; }
        public bool IsOutOfBounds { get; set; }
    }

    public static class Day24
    {
        static List<Hailstone> Hailstones = new List<Hailstone>();
        public static void Solve()
        {
            var rows = input.Split('\n').ToArray();
            ParseHailstones(rows);
            var count = GetCollidedHailstonesForPart1();
            Console.WriteLine(count);
        }

        static int GetCollidedHailstonesForPart1()
        {
            var min = 200000000000000;
            var max = 400000000000000;
            var simplifier = 1000000;
            //var min = 7;
            //var max = 27;
            var count = 0;
            
            for (int a = 0; a < Hailstones.Count-1; a++)
            {
                var aSegment = Hailstones[a];
                if (aSegment.IsOutOfBounds || (aSegment.IsOutOfBounds = IsHailstoneOutOfBounds(aSegment,min,max)))
                    continue;

                FindSegment(aSegment, min, max);
                if (aSegment.IsOutOfBounds)
                    continue;

                for (int b = a+1; b < Hailstones.Count; b++)
                {
                    var bSegment = Hailstones[b];
                    if (bSegment.IsOutOfBounds || (bSegment.IsOutOfBounds = IsHailstoneOutOfBounds(bSegment,min,max)))
                        continue;

                    FindSegment(bSegment, min, max);
                    if (bSegment.IsOutOfBounds)
                        continue;

                    if (ExtraMath.DoIntersect(new PointL(aSegment.XStart/simplifier, aSegment.YStart/simplifier), new PointL(aSegment.XEnd/simplifier, aSegment.YEnd/simplifier),
                                              new PointL(bSegment.XStart/simplifier, bSegment.YStart/simplifier), new PointL(bSegment.XEnd/simplifier, bSegment.YEnd/simplifier)))
                    {
                        count++;
                        //Console.WriteLine("Lines {0} and {1} intersect", (a+1).ConvertToLetterName(), (b+1).ConvertToLetterName());
                    }
                    //else
                        //Console.WriteLine("Lines {0} and {1} do not intersect", (a+1).ConvertToLetterName(), (b+1).ConvertToLetterName());
                }
            }

            return count;
        }

        static bool IsHailstoneOutOfBounds(Hailstone hailstone, long min, long max)
        {
            return (hailstone.X < min && hailstone.XChange <= 0) || 
                    (hailstone.X > max && hailstone.XChange >= 0) ||
                    (hailstone.Y < min && hailstone.YChange <= 0) || 
                    (hailstone.Y > max && hailstone.YChange >= 0);
        }

        static void FindSegment(Hailstone hailstone, long min, long max, bool includeZ = false)
        {
            if (hailstone.IsOutOfBounds || hailstone.SegmentCalculated)
                return;

            long xTimeToStart = 0, yTimeToStart = 0, zTimeToStart = 0;
            if (hailstone.X < min || hailstone.X > max)
                xTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.X) - Math.Min(hailstone.X, max))/(double)hailstone.XChange));
            if (hailstone.Y < min || hailstone.Y > max)
                yTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.Y) - Math.Min(hailstone.Y, max))/(double)hailstone.YChange));
            if (hailstone.Z < min || hailstone.Z > max)
                zTimeToStart = (long)Math.Abs(Math.Ceiling((Math.Max(min, hailstone.Z) - Math.Min(hailstone.Z, max))/(double)hailstone.ZChange));
            var timeToStart = Math.Max(xTimeToStart,yTimeToStart);
            if (includeZ)
                timeToStart = Math.Max(timeToStart,zTimeToStart);
            var xStart = hailstone.X + hailstone.XChange*timeToStart;
            var yStart = hailstone.Y + hailstone.YChange*timeToStart;
            var zStart = hailstone.Z + hailstone.ZChange*timeToStart;

            if (xStart < min || (max-hailstone.XChange) < xStart ||
                yStart < min || max-hailstone.YChange < yStart ||
                (includeZ && (zStart < min || max-hailstone.ZChange < zStart)))
            {
                hailstone.IsOutOfBounds = true;
                return;
            }

            var xLength = hailstone.XChange >= 0 ? max - xStart : xStart - min;
            var yLength = hailstone.YChange >= 0 ? max - yStart : yStart - min;
            var zLength = hailstone.ZChange >= 0 ? max - zStart : zStart - min;
            var xTimeToEnd = (long)Math.Abs(Math.Ceiling(xLength/(double)hailstone.XChange));
            var yTimeToEnd = (long)Math.Abs(Math.Ceiling(yLength/(double)hailstone.YChange));
            var zTimeToEnd = (long)Math.Abs(Math.Ceiling(zLength/(double)hailstone.ZChange));
            var timeToEnd = Math.Min(xTimeToEnd,yTimeToEnd);
            if (includeZ)
                timeToEnd = Math.Min(timeToEnd,zTimeToEnd);
            var xEnd = xStart + hailstone.XChange*timeToEnd;
            var yEnd = yStart + hailstone.YChange*timeToEnd;
            var zEnd = zStart + hailstone.ZChange*timeToEnd;



            hailstone.StartCoords = new Vector<long>(new [] { xStart, yStart, zStart, 0L });
            hailstone.EndCoords = new Vector<long>(new [] { xEnd, yEnd, zEnd, 0L });
            hailstone.SegmentCalculated = true;
        }

        static void ParseHailstones(string[] rows)
        {
            var zeroArray = new [] { 0L };
            foreach (var row in rows)
            {
                var data = row.Split('@');
                var coords = data[0].Trim().Split(", ").Select(long.Parse).Concat(zeroArray).ToArray();
                var velocity = data[1].Trim().Split(", ").Select(long.Parse).Concat(zeroArray).ToArray();
                Hailstones.Add(new Hailstone(new Vector<long>(coords), new Vector<long>(velocity)));
            }
        }

        static string input = @""; //paste it manually from the page
    }
}
