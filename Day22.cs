namespace AOC2023
{
    public struct Point3D
    {
        public Point3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(int[] xyz)
        {
            X = xyz[0];
            Y = xyz[1];
            Z = xyz[2];
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class Brick
    {
        public Brick(Point3D start, Point3D end)
        {
            Start = start;
            End = end;
            SupportedBy = new List<Brick>();
            Supports = new List<Brick>();
            Destroys = new List<Brick>();
            PartialDestruction = new List<Brick>();
        }
        public Point3D Start { get; }
        public Point3D End { get; }
        public List<Brick> SupportedBy { get; }
        public List<Brick> Supports { get; }
        public List<Brick> Destroys { get; set; }
        public List<Brick> PartialDestruction { get; set; }
        public bool DestructionMapped { get; set; }
    }

    public static class Day22
    {
        static List<Brick> Bricks = new List<Brick>();

        public static void Solve()
        {
            var brickText = input.Split('\n').Select(r => r.TrimEnd().Split('~')).ToArray();
            ParseBricks(brickText);
            PileBricks();

            SolvePart2();
        }

        public static void SolvePart2()
        {
            foreach (var brick in Bricks)
                DestroyBrick(brick);

            var destruction = Bricks.Sum(b => b.Destroys.Count);
            Console.WriteLine(destruction);
        }

        public static void SolvePart1()
        {
            var freeBricks = 0;
            foreach (var brick in Bricks)
                if (!IsSupportingAnotherBrick(brick))
                    freeBricks++;
            Console.WriteLine(freeBricks);
        }

        static List<Brick> DestroyBrick(Brick brick, List<Brick> alreadyDestroyedBricks = null)
        {
            var destroyedBricks = alreadyDestroyedBricks != null ? alreadyDestroyedBricks.ToList() : new List<Brick>();
            destroyedBricks.Add(brick);

            if (!brick.DestructionMapped)
            {
                foreach (var toDestroy in brick.Supports)
                {
                    if (toDestroy.SupportedBy.Count > 1)
                    {
                        brick.PartialDestruction.Add(toDestroy);
                        continue;
                    }
                    
                    brick.Destroys = DestroyBrick(toDestroy, brick.Destroys);
                }

                brick.DestructionMapped = true;
            }

            destroyedBricks.AddRange(brick.Destroys);
            destroyedBricks = destroyedBricks.Distinct().ToList();
            var partialDestruction = brick.PartialDestruction.ToList();
            foreach(var destroyedBrick in brick.Destroys)
            {
                partialDestruction.AddRange(destroyedBrick.PartialDestruction);
            }
            partialDestruction = partialDestruction.Distinct().ToList();

            foreach(var partialBrick in partialDestruction)
            {
                if (partialBrick.SupportedBy.All(destroyedBricks.Contains))
                    destroyedBricks = DestroyBrick(partialBrick, destroyedBricks).Distinct().ToList();
            }

            return destroyedBricks;
        }

        static bool IsSupportingAnotherBrick(Brick brick)
        {
            if (!brick.Supports.Any())
                return false;

            foreach (var topBrick in brick.Supports)
            {
                if (topBrick.SupportedBy.Count == 1)
                    return true;
            }

            return false;
        }

        static void ParseBricks(string[][] brickSnapshot)
        {
            foreach(var row in brickSnapshot)
            {
                var start = row[0].Split(',').Select(int.Parse).ToArray();
                var end = row[1].Split(',').Select(int.Parse).ToArray();
                Bricks.Add(new Brick(new Point3D(start), new Point3D(end)));
            }
            
            Bricks = Bricks.OrderBy(b => b.Start.Z).ToList();
        }

        static void PileBricks()
        {
            var lenX = Bricks.Max(b => b.End.X)+1;
            var lenY = Bricks.Max(b => b.End.Y)+1;

            var topFreeSpots = new int[lenX,lenY];
            var pileOfBricks = new List<Brick[,]>();
            foreach(var brick in Bricks)
            {
                var height = brick.End.Z - brick.Start.Z + 1;
                var zPos = 0;
                for (int x = brick.Start.X; x <= brick.End.X; x++)
                    for (int y = brick.Start.Y; y <= brick.End.Y; y++)
                        zPos = Math.Max(zPos, topFreeSpots[x,y]);

                for (int z = zPos; z < zPos+height; z++)
                {
                    if (pileOfBricks.Count - 1 < z)
                        pileOfBricks.Add(new Brick[lenX,lenY]);
                    
                    for (int x = brick.Start.X; x <= brick.End.X; x++)
                    {
                        for (int y = brick.Start.Y; y <= brick.End.Y; y++)
                        {
                            pileOfBricks[z][x,y] = brick;
                            topFreeSpots[x,y] = z+1;
                            if (z > 0)
                            {
                                var support = pileOfBricks[z-1][x,y];
                                if (support != null && support != brick && !brick.SupportedBy.Contains(support))
                                {
                                    brick.SupportedBy.Add(support);
                                    support.Supports.Add(brick);
                                }
                            }
                        }
                    }
                }
            }

        }

        static string input = @""; //paste it manually from the page
    }
}
