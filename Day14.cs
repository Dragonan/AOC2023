namespace AOC2023
{
    public class Rock
    {
        public const byte XAxis = 0;
        public const byte YAxis = 1;

        public byte[] Axes { get; set; }
        public byte X { get => Axes[XAxis]; set => Axes[XAxis] = value; }
        public byte Y { get => Axes[YAxis]; set => Axes[YAxis] = value; }

        public Rock(byte x, byte y)
        {
            Axes = new [] { x, y };
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }
    }
    public static class Day14
    {
        public static void Solve()
        {
            var dish = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();
            var heavyRocks = new List<Rock>();
            var walls = new List<Rock>();
            var rocksByRow = new List<List<Rock>>();
            var rocksByColumn = new List<List<Rock>>();
            var wallsByRow = new List<List<Rock>>();
            var wallsByColumn = new List<List<Rock>>();

            for (byte i = 0; i < dish.Length; i++)
            {
                rocksByRow.Add(new List<Rock>());
                wallsByRow.Add(new List<Rock>());

                for (byte j = 0; j < dish[i].Length; j++)
                {
                    if (i == 0)
                    {
                        rocksByColumn.Add(new List<Rock>());
                        wallsByColumn.Add(new List<Rock>());
                    }

                    if (dish[i][j] == 'O')
                    {
                        var rock = new Rock(j,i);
                        heavyRocks.Add(rock);
                        rocksByRow[i].Add(rock);
                        rocksByColumn[j].Add(rock);
                    }
                    else if (dish[i][j] == '#')
                    {
                        var wall = new Rock(j,i);
                        walls.Add(wall);
                        wallsByRow[i].Add(wall);
                        wallsByColumn[j].Add(wall);
                    }
                }
            }
            
            int[] endRockCounts = {};

            TiltToStart(wallsByColumn, rocksByColumn, rocksByRow, Rock.YAxis, Rock.XAxis); //north
            TiltToStart(wallsByRow, rocksByRow, rocksByColumn, Rock.XAxis, Rock.YAxis); //west
            TiltToEnd(wallsByColumn, rocksByColumn, rocksByRow, Rock.YAxis, Rock.XAxis); //south
            TiltToEnd(wallsByRow, rocksByRow, rocksByColumn, Rock.XAxis, Rock.YAxis); //east

            var unique = new List<(int[] Rows, int[] Columns)> { 
                (rocksByRow.Select(r => r.Count).ToArray(), 
                 rocksByColumn.Select(r => r.Count).ToArray())};
            
            var cycles = 1000000000;
            for (int i = 1; i < cycles; i++)
            {
                TiltToStart(wallsByColumn, rocksByColumn, rocksByRow, Rock.YAxis, Rock.XAxis); //north
                TiltToStart(wallsByRow, rocksByRow, rocksByColumn, Rock.XAxis, Rock.YAxis); //west
                TiltToEnd(wallsByColumn, rocksByColumn, rocksByRow, Rock.YAxis, Rock.XAxis); //south
                TiltToEnd(wallsByRow, rocksByRow, rocksByColumn, Rock.XAxis, Rock.YAxis); //east

                var currentRows = rocksByRow.Select(r => r.Count).ToArray();
                var currentColumns = rocksByColumn.Select(r => r.Count).ToArray();

                var s = unique.TakeWhile(u => !u.Rows.SequenceEqual(currentRows) || 
                                              !u.Columns.SequenceEqual(currentColumns))
                              .Count();

                if (s < unique.Count)
                {
                    var cycleLength = i - s;
                    var leftover = (cycles - s) % cycleLength;
                    var uniquePatternAtEnd = leftover == 0 ? (i - 1) : (s - 1 + leftover);
                    endRockCounts = unique[uniquePatternAtEnd].Rows;
                    break;
                }

                unique.Add((currentRows, currentColumns));
            }

            var sum = 0;
            for (int i = 0, j = endRockCounts.Length; i < endRockCounts.Length && j > 0; i++, j--)
            {
                sum += j*endRockCounts[i];
            }
            Console.WriteLine(sum);
        }

        static void TiltToStart(List<List<Rock>> wallLines, List<List<Rock>> rockLines, List<List<Rock>> rockHomes, byte axisToMove, byte axisToCompare)
        {
            for (int i = 0; i < wallLines.Count; i++)
            {
                var previousWall = -1;
                for (int j = 0; j <= wallLines[i].Count; j++)
                {
                    var nextWall = j == wallLines[i].Count ? rockHomes.Count : wallLines[i][j].Axes[axisToMove];
                    var rocksToMove = GetRocksToMove(rockLines[i], axisToMove, previousWall, nextWall);

                    if (rocksToMove.Any() && nextWall - previousWall - 1 > rocksToMove.Count)
                    {
                        var lowestIndex = previousWall + 1;
                        var highestIndex = lowestIndex + rocksToMove.Count - 1;
                        var filledSpots = new List<byte>();
                        for (int ri = 0, ni = lowestIndex; ri < rocksToMove.Count; ri++, ni++)
                        {
                            if (rocksToMove[ri].Axes[axisToMove] <= highestIndex)
                            {
                                filledSpots.Add(rocksToMove[ri].Axes[axisToMove]);
                                ni--;
                                continue;
                            }

                            while (filledSpots.Any() && filledSpots[0] == ni)
                            {
                                filledSpots.RemoveAt(0);
                                ni++;
                            }

                            MoveRock(rocksToMove[ri], rockHomes, (byte)ni, axisToMove, axisToCompare);
                        }

                    }
                    previousWall = nextWall;
                }
            }
        }

        static void TiltToEnd(List<List<Rock>> wallLines, List<List<Rock>> rockLines, List<List<Rock>> rockHomes, byte axisToMove, byte axisToCompare)
        {
            for (int i = 0; i < wallLines.Count; i++)
            {
                var previousWall = -1;
                for (int j = 0; j <= wallLines[i].Count; j++)
                {
                    var nextWall = j == wallLines[i].Count ? rockHomes.Count : wallLines[i][j].Axes[axisToMove];
                    var rocksToMove = GetRocksToMove(rockLines[i], axisToMove, previousWall, nextWall);

                    if (rocksToMove.Any() && nextWall - previousWall - 1 > rocksToMove.Count)
                    {
                        var lowestIndex = nextWall - rocksToMove.Count;
                        var highestIndex = nextWall - 1;
                        var filledSpots = new List<byte>();
                        for (int ri = rocksToMove.Count - 1, ni = highestIndex; ri >= 0; ri--, ni--)
                        {
                            if (rocksToMove[ri].Axes[axisToMove] >= lowestIndex)
                            {
                                filledSpots.Add(rocksToMove[ri].Axes[axisToMove]);
                                ni++;
                                continue;
                            }

                            while (filledSpots.Any() && filledSpots[0] == ni)
                            {
                                filledSpots.RemoveAt(0);
                                ni--;
                            }

                            MoveRock(rocksToMove[ri], rockHomes, (byte)ni, axisToMove, axisToCompare);
                        }

                    }
                    previousWall = nextWall;
                }
            }
        }

        static List<Rock> GetRocksToMove(List<Rock> rocks, int axisToMove, int previousWall, int nextWall)
        {
            var result = new List<Rock>();
            for (int i = 0; i < rocks.Count; i++)
            {
                if (rocks[i].Axes[axisToMove] < previousWall)
                    continue;
                else if (rocks[i].Axes[axisToMove] > nextWall)
                    break;
                result.Add(rocks[i]);
            }
            return result;
        }

        static void MoveRock(Rock rock, List<List<Rock>> rockHomes, byte newIndex, byte axisToMove, byte axisToCompare)
        {
            var row = rockHomes[rock.Axes[axisToMove]];
            row.Remove(rock);
            rock.Axes[axisToMove] = newIndex;
            row = rockHomes[newIndex];
            var inserted = false;
            for (byte k = 0; k < row.Count; k++)
            {
                if (row[k].Axes[axisToCompare] < rock.Axes[axisToCompare])
                    continue;

                row.Insert(k, rock);
                inserted = true;
                break;
            }
            if (!inserted)
                row.Add(rock);
        }

        static void SolvePart1()
        {
            var rows = input.Split('\n').Select(r => r.ToCharArray()).ToArray();

            var heavyRocks = new int[rows.Length];
            heavyRocks[0] = rows[0].Count(c => c == 'O');

            for (int i = 1; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    if (rows[i][j] == 'O')
                    {
                        var e = i;
                        while (e > 0 && rows[e-1][j] == '.')
                        {
                            rows[e-1][j] = 'O';
                            rows[e][j] = '.';
                            e--;
                        }
                        heavyRocks[e]++;
                    }
                }
            }

            var sum = 0;
            for (int i = 0, j = heavyRocks.Length; i < heavyRocks.Length && j > 0; i++, j--)
            {
                sum += j*heavyRocks[i];
            }

            Console.WriteLine(sum);
        }

        static string input = @""; //paste it manually from the page
    }
}