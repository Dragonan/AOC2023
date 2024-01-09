using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics;

namespace AOC2023
{
    public static class ExtraMath
    {
        public static long FindLCM(long[] numbers)
        {
            var lcm = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
                lcm = lcm * numbers[i] / FindGCD(lcm, numbers[i]);
            return lcm;
        }

        public static long FindGCD(long a, long b)
        {
            if (b == 0)
                return a;
            return FindGCD(b, a % b);
        }

        public static long SumNNaturalNumbers(long n)
        {
            return n*(n+1)/2;
        }

        public static long SumAllOddNumbers(long max)
        {
            if (max % 2 == 1)
                max++;

            var n = max/2;

            return (long)Math.Pow(n,2);

        }

        public static long SumAllEvenNumbers(long max)
        {
            if (max % 2 == 1)
                max--;
            
            var n = max/2;
            return n*(n+1);
        }

        // Given three collinear points p, q, r, the function checks if 
        // point q lies on line segment 'pr' 
        public static bool OnSegment(PointL p, PointL q, PointL r) 
        { 
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) && 
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y)) 
            return true; 
        
            return false; 
        } 
  
        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are collinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        public static long Orientation(PointL p, PointL q, PointL r) 
        { 
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
            // for details of below formula. 
            var val = (q.Y - p.Y) * (r.X - q.X) - 
                    (q.X - p.X) * (r.Y - q.Y); 
        
            if (val == 0) return 0; // collinear 
        
            return (val > 0)? 1: 2; // clock or counterclock wise 
        } 
  
        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        public static bool DoIntersect(PointL p1, PointL q1, PointL p2, PointL q2) 
        { 
            // Find the four orientations needed for general and 
            // special cases 
            var o1 = Orientation(p1, q1, p2); 
            var o2 = Orientation(p1, q1, q2); 
            var o3 = Orientation(p2, q2, p1); 
            var o4 = Orientation(p2, q2, q1); 
        
            // General case 
            if (o1 != o2 && o3 != o4) 
                return true; 
        
            // Special Cases 
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1 
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true; 
        
            // p1, q1 and q2 are collinear and q2 lies on segment p1q1 
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true; 
        
            // p2, q2 and p1 are collinear and p1 lies on segment p2q2 
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true; 
        
            // p2, q2 and q1 are collinear and q1 lies on segment p2q2 
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true; 
        
            return false; // Doesn't fall in any of the above cases 
        } 

        /// <summary>
        /// This is based off an explanation and expanded math presented by Paul Bourke:
        /// 
        /// It takes two lines as inputs and returns true if they intersect, false if they 
        /// don't.
        /// If they do, ptIntersection returns the point where the two lines intersect.  
        /// </summary>
        /// <param name="L1">The first line</param>
        /// <param name="L2">The second line</param>
        /// <param name="ptIntersection">The point where both lines intersect (if they do).</param>
        /// <returns></returns>
        /// <remarks>See http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/</remarks>
        public static bool DoLinesIntersect(PointL P1, PointL P2, PointL P3, PointL P4, ref PointL ptIntersection)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d =
                (P4.Y - P3.Y) * (P2.X - P1.X)
                -
                (P4.X - P3.X) * (P2.Y - P1.Y);

            //n_a and n_b are calculated as seperate values for readability
            double n_a =
                (P4.X - P3.X) * (P1.Y - P3.Y)
                -
                (P4.Y - P3.Y) * (P1.X - P3.X);

            double n_b =
                (P2.X - P1.X) * (P1.Y - P3.Y)
                -
                (P2.Y - P1.Y) * (P1.X - P3.X);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.  
            // If n_a and n_b were both equal to zero the lines would be on top of each 
            // other (coincidental).  This check is not done because it is not 
            // necessary for this implementation (the parallel check accounts for this).
            if (d == 0)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            double ua = n_a / d;
            double ub = n_b / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                ptIntersection.X = P1.X + ((long)ua * (P2.X - P1.X));
                ptIntersection.Y = P1.Y + ((long)ua * (P2.Y - P1.Y));
                return true;
            }
            return false;
        }


        /// <summary>
        /// Calculates the intersection line segment between 2 lines (not segments).
        /// Returns false if no solution can be found.
        /// </summary>
        /// <returns></returns>
        public static bool CalculateLineLineIntersection(Vector<long> line1Point1, Vector<long> line1Point2, 
            Vector<long> line2Point1, Vector<long> line2Point2, out Vector<long> resultSegmentPoint1, out Vector<long> resultSegmentPoint2)
        {
            // Algorithm is ported from the C algorithm of 
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            resultSegmentPoint1 = Vector<long>.Zero;
            resultSegmentPoint2 = Vector<long>.Zero;
            
            var p1 = line1Point1;
            var p2 = line1Point2;
            var p3 = line2Point1;
            var p4 = line2Point2;
            var p13 = p1 - p3;
            var p43 = p4 - p3;
            
            if (p43.DistanceSquared() < Math.E) {
                return false;
            }
            Vector3 p21 = p2 - p1;
            if (p21.LengthSq() < Math.Epsilon) {
                return false;
            }
            
            double d1343 = p13.X * (double)p43.X + (double)p13.Y * p43.Y + (double)p13.Z * p43.Z;
            double d4321 = p43.X * (double)p21.X + (double)p43.Y * p21.Y + (double)p43.Z * p21.Z;
            double d1321 = p13.X * (double)p21.X + (double)p13.Y * p21.Y + (double)p13.Z * p21.Z;
            double d4343 = p43.X * (double)p43.X + (double)p43.Y * p43.Y + (double)p43.Z * p43.Z;
            double d2121 = p21.X * (double)p21.X + (double)p21.Y * p21.Y + (double)p21.Z * p21.Z;
            
            double denom = d2121 * d4343 - d4321 * d4321;
            if (Math.Abs(denom) < Math.Epsilon) {
                return false;
            }
            double numer = d1343 * d4321 - d1321 * d4343;
            
            double mua = numer / denom;
            double mub = (d1343 + d4321 * (mua)) / d4343;
            
            resultSegmentPoint1.X = (float)(p1.X + mua * p21.X);
            resultSegmentPoint1.Y = (float)(p1.Y + mua * p21.Y);
            resultSegmentPoint1.Z = (float)(p1.Z + mua * p21.Z);
            resultSegmentPoint2.X = (float)(p3.X + mub * p43.X);
            resultSegmentPoint2.Y = (float)(p3.Y + mub * p43.Y);
            resultSegmentPoint2.Z = (float)(p3.Z + mub * p43.Z);

            return true;
        }

    }
}