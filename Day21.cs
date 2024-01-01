using System.Drawing;

namespace AOC2023
{
    public static class Day21
    {
        static char[][] Map;
        public static void Solve()
        {
            Map = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();

            SolvePart2();
        }

        public static void SolvePart2()
        {
            var totalSteps = 26501365;
            var halfLength = (int)Math.Floor(Map.Length/2d); //65
            var stepsToFillMap = Map.Length + halfLength; //196
            var stepsToFillEvenPartsOfMap = totalSteps % 2 == 1 && stepsToFillMap % 2 == 1 ? stepsToFillMap : stepsToFillMap - 1; //195
            var stepsToFillOddPartsOfMap = stepsToFillEvenPartsOfMap + 1; //196
            var farEdge = Map.Length - 1; //130
            var totalFilledLength = (int)Math.Floor((totalSteps - halfLength - 1d)/Map.Length);
            var leftoverSteps = ((totalSteps-halfLength) % Map.Length) + Map.Length - 1; //130
            var outerLineSteps = leftoverSteps - halfLength - 1; //64
            var innerLineSteps = 2*farEdge - outerLineSteps - 1; //195
            var totalReach = 0L;
            
            //calculate infill
            var start = FindStartingPoint();
            var evenInfillReach = GetReach(start, stepsToFillEvenPartsOfMap);
            var oddInfillReach = GetReach(start, stepsToFillOddPartsOfMap);
            totalReach += evenInfillReach;
            totalReach += 4*evenInfillReach*ExtraMath.SumAllEvenNumbers(totalFilledLength);
            totalReach += 4*oddInfillReach*ExtraMath.SumAllOddNumbers(totalFilledLength);

            //calculate points
            var points = new Point[] 
            { 
                new (0, halfLength), new (farEdge, halfLength), 
                new (halfLength, 0), new (halfLength, farEdge) 
            };
            var pointsReach = 0L;
            foreach (var startPoint in points)
            {
                pointsReach += GetReach(startPoint, leftoverSteps);
                if (leftoverSteps >= Map.Length)
                    pointsReach += GetReach(startPoint, leftoverSteps - Map.Length);
            }
            totalReach += pointsReach;

            //calculate lines
            var lines = new Point[]
            {
                new (0, 0), new (farEdge, farEdge),
                new (0, farEdge), new (farEdge, 0)
            };
            var linesReach = 0L;
            foreach (var startPoint in lines)
            {
                linesReach += (totalFilledLength+1)*GetReach(startPoint, outerLineSteps);
                linesReach += totalFilledLength*GetReach(startPoint, innerLineSteps);
            }
            totalReach += linesReach;

            Console.WriteLine(totalReach);
        }

        public static void SolvePart1()
        {
            var start = FindStartingPoint();
            var reach = GetReach(start, 64);
            Console.WriteLine(reach);
        }

        static int GetReach(Point start, int steps)
        {
            if (steps == 0)
                return 1;
            var paths = GetLegalMoves(start);
            for (int i = 1; i < steps; i++)
            {
                var nextMoves = new List<Point>();
                for (int j = 0; j < paths.Count; j++)
                {
                    var path = paths[j];
                    nextMoves.AddRange(GetLegalMoves(path, nextMoves));
                }
                paths = nextMoves;
            }
            return paths.Count;
        }

        static List<Point> GetLegalMoves(Point point, List<Point> forbiddenPoints = null)
        {
            var result = new List<Point>();
            var hasForbiddenPoints = forbiddenPoints != null && forbiddenPoints.Any();
            var adjacent = new [] 
            {
                new Point(point.X, point.Y - 1),
                new Point(point.X, point.Y + 1),
                new Point(point.X - 1, point.Y),
                new Point(point.X + 1, point.Y)
            };

            foreach (var adj in adjacent)
            {
                if (!adj.IsOutOfBounds(Map.Length)
                    && Map[adj.Y][adj.X] != '#' 
                    && (!hasForbiddenPoints || !forbiddenPoints.Contains(adj)))
                    result.Add(adj);
            }

            return result;
        }

        static Point FindStartingPoint()
        {
            for (int y = 0; y < Map.Length; y++)
                for (int x = 0; x < Map[y].Length; x++)
                    if (Map[y][x] == 'S')
                        return new Point(x, y);
            
            return new Point(0, 0);
        }

        static string input = @""; //paste it manually from the page
    }
}