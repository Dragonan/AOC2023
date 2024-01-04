using System.Drawing;

namespace AOC2023
{
    public class Crossroad
    {
        public Crossroad(string name, Point coords)
        {
            Name = name;
            Coords = coords;
            Inbound = new List<Trail>();
            Outbound = new List<Trail>();
        }

        public string Name { get; }
        public Point Coords { get; }
        public List<Trail> Inbound { get; }
        public List<Trail> Outbound { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Trail
    {
        public Trail(Crossroad start)
        {
            Start = start;
        }

        public Crossroad Start { get; }
        public Crossroad End { get; private set; }
        public int Length { get; private set; }

        public void SetEndpoint(Crossroad end, int length)
        {
            End = end;
            Length = length;
            Start.Outbound.Add(this);
            End.Inbound.Add(this);
        }

        public override string ToString()
        {
            return Start.Name + '-' + End.Name;
        }
    }

    public class TrailRoute
    {
        public TrailRoute()
        {
            PassedCrossroads = new List<Crossroad>();
        }

        public TrailRoute(Crossroad crossroad) : this()
        {
            PassedCrossroads.Add(crossroad);
            Length = 0;
        }

        public TrailRoute(TrailRoute original)
        {
            PassedCrossroads = original.PassedCrossroads.ToList();
            Length = original.Length;
        }

        public List<Crossroad> PassedCrossroads { get; }
        public int Length { get; set; }

        public override string ToString()
        {
            return Length.ToString();
        }
    }

    public static class Day23
    {
        static List<Trail> Trails = new List<Trail>();
        static List<Crossroad> Crossroads = new List<Crossroad>();

        public static void Solve()
        {
            var map = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();
            ParseMap(map);

            var lengths = GetRoutesForPart2(Crossroads.First());
            Console.WriteLine(lengths.Max() - 1);
        }

        static List<int> GetRoutesForPart1(Crossroad crossroad, int lengthSoFar = 0)
        {
            if (!crossroad.Outbound.Any())
                return new List<int> { lengthSoFar };

            var result = new List<int>();
            foreach (var branch in crossroad.Outbound)
                result.AddRange(GetRoutesForPart1(branch.End, lengthSoFar + branch.Length + 1));
            
            return result;
        }

        static List<int> GetRoutesForPart2(Crossroad crossroad, TrailRoute route = null)
        {
            route ??= new TrailRoute(crossroad);

            if (!crossroad.Outbound.Any())
                return new List<int> { route.Length };
            
            var trailsToExplore = crossroad.Inbound.Where(t => !route.PassedCrossroads.Contains(t.Start)).Select(t => new {t.Length, OtherEnd = t.Start})
                        .Concat(crossroad.Outbound.Where(t => !route.PassedCrossroads.Contains(t.End)).Select(t => new {t.Length, OtherEnd = t.End}));

            var result = new List<int>();
            if (!trailsToExplore.Any())
                return result;

            foreach (var trail in trailsToExplore)
            {
                var newRoute = new TrailRoute(route);
                newRoute.Length += trail.Length + 1;
                newRoute.PassedCrossroads.Add(trail.OtherEnd);
                result.AddRange(GetRoutesForPart2(trail.OtherEnd, newRoute));
            }

            return result;
        }

        static void ParseMap(char[][] map)
        {
            var entrance = new Crossroad("SSS", new (1,0));
            Crossroads.Add(entrance);
            var startTrail = new Trail(entrance);
            var toExplore = new List<(Trail Trail, Point StartPoint)> { (startTrail, entrance.Coords) };

            while(toExplore.Any())
            {
                var toFollow = toExplore.ToList();
                toExplore.Clear();
                foreach (var path in toFollow)
                {
                    var lastPoint = path.StartPoint;
                    var nextPoints = new List<Point> { lastPoint };
                    var trailLength = -1;
                    var isMerge = false;
                    while (!isMerge && nextPoints.Count == 1)
                    {
                        trailLength++;
                        var currentPoint = nextPoints[0];
                        nextPoints.Clear();
                        var surroundings = new (Point Coords, char AllowedSlope)[] {
                            new (new (currentPoint.X, currentPoint.Y-1), '^'),
                            new (new (currentPoint.X, currentPoint.Y+1), 'v'),
                            new (new (currentPoint.X-1, currentPoint.Y), '<'),
                            new (new (currentPoint.X+1, currentPoint.Y), '>')
                        };

                        var currentSpot = map[currentPoint.Y][currentPoint.X];
                        if (currentSpot != '.')
                        {
                            surroundings = surroundings.Where(s => s.AllowedSlope == currentSpot).ToArray();
                        }

                        foreach (var surrounding in surroundings)
                        {
                            if (lastPoint != surrounding.Coords && !surrounding.Coords.IsOutOfBounds(map.Length))
                            {
                                var spot = map[surrounding.Coords.Y][surrounding.Coords.X];
                                if (spot == '.' || spot == surrounding.AllowedSlope)
                                    nextPoints.Add(surrounding.Coords);
                                else if (spot != '#')
                                    isMerge = true;

                            }
                        }
                        lastPoint = currentPoint;
                    }

                    var endCrossroad = Crossroads.FirstOrDefault(t => t.Coords == lastPoint);
                    if (endCrossroad == null)
                    {
                        endCrossroad = new Crossroad(Crossroads.Count.ConvertToLetterName(), lastPoint);
                        Crossroads.Add(endCrossroad);
                    }
                    path.Trail.SetEndpoint(endCrossroad, trailLength);
                    Trails.Add(path.Trail);
                    
                    if (!Trails.Any(t => t.Start == endCrossroad) && !toExplore.Any(t => t.Trail.Start == endCrossroad))
                        foreach (var branchStart in nextPoints)
                            toExplore.Add((new Trail(endCrossroad), branchStart));
                }
            }
        }

        static string input = @""; //paste it manually from the page
    }
}
