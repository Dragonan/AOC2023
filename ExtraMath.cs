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
    }
}
