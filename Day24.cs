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

    //These calculations, both MatrixMath and LinearSolver, are copied from https://github.com/tmbarker
    public static class MatrixMath
    {
        public static decimal Intersect3D(IReadOnlyList<Hailstone> hailstones)
        {
            //  Let:
            //    <p_rock>(t) = <X,Y,Z> + t <DX,DY,DZ> 
            //    <p_hail>(t) = <x,y,z> + t <dx,dy,dz>
            
            //  A rock-hail collision requires the following to be true:
            //    X + t DX = x + t dx
            //    Y + t DY = y + t dy
            //    Z + t DZ = z + t dz
            
            //  Which implies:
            //    t = (X-x)/(dx-DX)
            //    t = (Y-y)/(dy-DY)
            //    t = (Z-z)/(dz-DZ)
            
            //  Equating the first two equalities from above yields:
            //    (X-x)/(dx-DX) = (Y-y)/(dy-DY)
            //    (X-x) (dy-DY) = (Y-y) (dx-DX)
            //    X*dy - X*DY - x*dy + x*DY = Y*dx - Y*DX - y*dx + y*DX
            //    Y*DX - X*DY = Y*dx - y*dx + y*DX - X*dy + x*dy - x*DY
            
            //  Note that the LHS of the above equation is true for any hail stone. Evaluating
            //  the RHS again for a different hailstone, and setting the two RHS equal, yields 
            //  the first of the below equations:
            
            //  (dy'-dy) X + (dx-dx') Y + (y-y') DX + (x'-x) DY =  x' dy' - y' dx' - x dy + y dx
            //  (dz'-dz) X + (dx-dx') Z + (z-z') DX + (x'-x) DZ =  x' dz' - z' dx' - x dz + z dx
            //  (dz-dz') Y + (dy'-dy) Z + (z'-z) DY + (y-y') DZ = -y' dz' + z' dy' + y dz - z dy
            
            //  The second and third are yielded by repeating the above process with X & Z, and 
            //  then Y & Z. This is a system of equations with 6 unknowns. Using two different
            //  pairs of hailstones (e.g. three total hailstones) yields 6 equations with 6
            //  unknowns, which we can now solve relatively trivially using linear algebra.
            
            var matrix = new decimal[6, 7];
            FillRow(matrix, row: 0, vals: Coefficients1(a: hailstones[0], b: hailstones[1]));
            FillRow(matrix, row: 1, vals: Coefficients1(a: hailstones[0], b: hailstones[2]));
            FillRow(matrix, row: 2, vals: Coefficients2(a: hailstones[0], b: hailstones[1]));
            FillRow(matrix, row: 3, vals: Coefficients2(a: hailstones[0], b: hailstones[2]));
            FillRow(matrix, row: 4, vals: Coefficients3(a: hailstones[0], b: hailstones[1]));
            FillRow(matrix, row: 5, vals: Coefficients3(a: hailstones[0], b: hailstones[2]));

            return LinearSolver
                .Solve(a: matrix)
                .Take(3)
                .Select(v => Math.Round(v))
                .Sum();
        }

        private static void FillRow(decimal[,] matrix, int row, decimal[] vals)
        {
            for (var j = 0; j < vals.Length; j++)
            {
                matrix[row, j] = vals[j];
            }
        }
        private static decimal[] Coefficients1(Hailstone a, Hailstone b)
        {
            // (dy'-dy) X + (dx-dx') Y + (y-y') DX + (x'-x) DY =  x' dy' - y' dx' - x dy + y dx
            var arr = new decimal[7];
            arr[0] = b.YChange - a.YChange;
            arr[1] = a.XChange - b.XChange;
            arr[3] = a.Y - b.Y;
            arr[4] = b.X - a.X;
            arr[6] = b.X * b.YChange - b.Y * b.XChange - a.X * a.YChange + a.Y * a.XChange;
            return arr;
        }
        
        private static decimal[] Coefficients2(Hailstone a, Hailstone b)
        {
            // (dz'-dz) X + (dx-dx') Z + (z-z') DX + (x'-x) DZ =  x' dz' - z' dx' - x dz + z dx
            var arr = new decimal[7];
            arr[0] = b.ZChange - a.ZChange;
            arr[2] = a.XChange - b.XChange;
            arr[3] = a.Z - b.Z;
            arr[5] = b.X - a.X;
            arr[6] = b.X * b.ZChange - b.Z * b.XChange - a.X * a.ZChange + a.Z * a.XChange;
            return arr;
        }
        
        private static decimal[] Coefficients3(Hailstone a, Hailstone b)
        {
            // (dz-dz') Y + (dy'-dy) Z + (z'-z) DY + (y-y') DZ = -y' dz' + z' dy' + y dz - z dy
            var arr = new decimal[7];
            arr[1] = a.ZChange - b.ZChange;
            arr[2] = b.YChange - a.YChange;
            arr[4] = b.Z - a.Z;
            arr[5] = a.Y - b.Y;
            arr[6] = -b.Y * b.ZChange + b.Z * b.YChange + a.Y * a.ZChange - a.Z * a.YChange;
            return arr;
        }
    }

    /// <summary>
    /// A generic utility class for solving linear systems of equations
    /// </summary>
    public static class LinearSolver
    {
        /// <summary>
        /// Solve a linear system of equations specified by an augmented coefficient matrix.
        /// </summary>
        /// <param name="a">The augmented coefficient matrix</param>
        /// <typeparam name="T">The type associated with each matrix element</typeparam>
        /// <returns>A vector containing a solution to the system of equations</returns>
        public static T[] Solve<T>(T[,] a) where T : INumber<T>
        {
            var n = a.GetLength(dimension: 0);
            var x = new T[n];
            PartialPivot(a, n);
            BackSubstitute(a, n, x);
            return x;
        }

        private static void PartialPivot<T>(T[,] a, int n) where T : INumber<T>
        {
            for (var i = 0; i < n; i++) 
            {
                var pivotRow = i;
                for (var j = i + 1; j < n; j++) {
                    if (T.Abs(a[j, i]) > T.Abs(a[pivotRow, i])) {
                        pivotRow = j;
                    }
                }
                
                if (pivotRow != i) {
                    for (var j = i; j <= n; j++) 
                    {
                        (a[i, j], a[pivotRow, j]) = (a[pivotRow, j], a[i, j]);
                    }
                }
                
                for (var j = i + 1; j < n; j++) 
                {
                    var factor = a[j, i] / a[i, i];
                    for (var k = i; k <= n; k++) 
                    {
                        a[j, k] -= factor * a[i, k];
                    }
                }
            }
        }
        
        private static void BackSubstitute<T>(T[,] a, int n, T[] x) where T : INumber<T>
        {
            for (var i = n - 1; i >= 0; i--)
            {
                var sum = T.Zero;
                for (var j = i + 1; j < n; j++)
                {
                    sum += a[i, j] * x[j];
                }
                x[i] = (a[i, n] - sum) / a[i, i];
            }
        }
    }

    public static class Day24
    {
        static List<Hailstone> Hailstones = new List<Hailstone>();
        public static void Solve()
        {
            var rows = input.Split('\n').ToArray();
            ParseHailstones(rows);
            // var count = GetCollidedHailstonesForPart1();
            // Console.WriteLine(count);
            var sum = FindThePerfectShotForPart2();
            Console.WriteLine(sum);
        }

        static long FindThePerfectShotForPart2()
        {
            return (long)MatrixMath.Intersect3D(Hailstones);
        }

        static int GetCollidedHailstonesForPart1()
        {
            var min = 200000000000000;
            var max = 400000000000000;
            var simplifier = 100000000000;
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

                    var p1 = new PointL(aSegment.XStart/simplifier, aSegment.YStart/simplifier);
                    var p2 = new PointL(aSegment.XEnd/simplifier, aSegment.YEnd/simplifier);
                    var p3 = new PointL(bSegment.XStart/simplifier, bSegment.YStart/simplifier);
                    var p4 = new PointL(bSegment.XEnd/simplifier, bSegment.YEnd/simplifier);

                    if (ExtraMath.DoIntersect(p1,p2,p3,p4))
                        count++;
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
