using System.Numerics;

namespace AOC2023
{
    public class Hailstone
    {
        public Hailstone(Vector<long> coords, Vector<long> velocity)
        {
            Coords = coords;
            Velocity = velocity;
            EndCoords = Vector<long>.Zero;
        }

        public Vector<long> Coords { get; }
        public Vector<long> Velocity { get; }
        public long X => Coords[0];
        public long Y => Coords[1];
        public long Z => Coords[2];
        public long XChange => Velocity[0];
        public long YChange => Velocity[1];
        public long ZChange => Velocity[2];
        public Vector<long> EndCoords { get; set; }
        public long XEnd => EndCoords[0];
        public long YEnd => EndCoords[1];
        public long ZEnd => EndCoords[2];
        public bool EndCalculated { get; set; }
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
            var min = 7;
            var max = 27;
            var count = 0;
            
            for (int a = 0; a < Hailstones.Count-1; a++)
            {
                var aSegment = Hailstones[a];
                FindLimits(aSegment, min, max);
                for (int b = a+1; b < Hailstones.Count; b++)
                {
                    var bSegment = Hailstones[b];
                    FindLimits(bSegment, min, max);
                    
                    
                    var dxA = aSegment.XEnd - aSegment.X;
                    var dxB = bSegment.XEnd - bSegment.X;
                    var dyA = aSegment.YEnd - aSegment.Y;
                    var dyB = bSegment.YEnd - bSegment.Y;
                    var p0 = dyB*(bSegment.XEnd - aSegment.X) - dxB*(bSegment.YEnd - aSegment.Y);
                    var p1 = dyB*(bSegment.XEnd - aSegment.XEnd) - dxB*(bSegment.YEnd - aSegment.YEnd);
                    var p2 = dyA*(aSegment.XEnd - bSegment.X) - dxA*(aSegment.YEnd - bSegment.Y);
                    var p3 = dyA*(aSegment.XEnd - bSegment.XEnd) - dxA*(aSegment.YEnd - bSegment.YEnd);

                    if (p0 * p1 <= 0 && p2 * p3 <= 0)
                        count++;
                }
            }

            return count;
        }

        static void FindLimits(Hailstone hailstone, long min, long max, bool includeZ = false)
        {
            if (hailstone.EndCalculated)
                return;

            var minX = hailstone.XChange >= 0 ? hailstone.X : min;
            var maxX = hailstone.XChange >= 0 ? max : hailstone.X;
            var minY = hailstone.YChange >= 0 ? hailstone.Y : min;
            var maxY = hailstone.YChange >= 0 ? max : hailstone.Y;
            var minZ = hailstone.ZChange >= 0 ? hailstone.Z : min;
            var maxZ = hailstone.ZChange >= 0 ? max : hailstone.Z;
            var xTime = (long)Math.Ceiling((maxX - minX)/(double)hailstone.XChange);
            var yTime = (long)Math.Ceiling((maxY - minY)/(double)hailstone.ZChange);
            var zTime = (long)Math.Ceiling((maxZ - minZ)/(double)hailstone.ZChange);
            var time = Math.Min(xTime,yTime);
            if (includeZ)
                time = Math.Min(time,zTime);
            
            var xEnd = hailstone.X + hailstone.XChange*time;
            var yEnd = hailstone.Y + hailstone.YChange*time;
            var zEnd = hailstone.Z + hailstone.ZChange*time;

            hailstone.EndCoords = new Vector<long>(new [] { xEnd, yEnd, zEnd, 0L });
            hailstone.EndCalculated = true;
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
