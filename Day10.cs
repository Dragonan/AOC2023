using System.Drawing;

namespace AOC2023
{
    public static class Day10
    {
        static Point Left = new Point(-1,0);
        static Point Right = new Point(1,0);
        static Point Up = new Point(0,-1);
        static Point Down = new Point(0,1);

        public static void Solve()
        {
            var pipeland = input.Split('\n').ToArray();

            var start = new Point(0,0);
            for (int y = 0; y < pipeland.Length; y++)
            {
                var x = pipeland[y].IndexOf('S');
                if (x != -1)
                {
                    start.X = x;
                    start.Y = y;
                    break;
                }
            }

            var pipeTypes = new (char Symbol, Point ExitA, Point ExitB)[]
            {
                ('F',Down,Right),             ('7',Left,Down),
                ('|',Up,Down),
                ('L',Up,Right),('-',Left,Right),('J',Up,Left)
            };

            var startNeighbours = new (string Symbols, Point Coords, Point LastExit)[] {
                ("7|F", new Point(start.X,start.Y-1), Up),
                ("L-F", new Point(start.X-1,start.Y), Left),
                ("J-7", new Point(start.X+1,start.Y), Right),
                ("J|L", new Point(start.X,start.Y+1), Down),
            };

            var currentPipe = start;
            var lastExit = Up;
            foreach (var neighbour in startNeighbours)
                if (neighbour.Coords.X >= 0 &&  neighbour.Coords.X < pipeland[0].Length && 
                    neighbour.Coords.Y >= 0 && neighbour.Coords.Y < pipeland.Length)
                {
                    var symbol = pipeland[neighbour.Coords.Y][neighbour.Coords.X];
                    if (neighbour.Symbols.Contains(symbol))
                    {
                        currentPipe = neighbour.Coords;
                        lastExit = neighbour.LastExit;
                        break;
                    }
                }

            var loop = new List<Point>{ start };
            while (currentPipe.X != start.X || currentPipe.Y != start.Y)
            {
                loop.Add(currentPipe);
                var symbol = pipeland[currentPipe.Y][currentPipe.X];
                var pipe = pipeTypes.First(p => p.Symbol == symbol);
                var exit = pipe.ExitA;
                if (exit.X == lastExit.X*-1 && exit.Y == lastExit.Y*-1)
                    exit = pipe.ExitB;
                currentPipe = new Point(currentPipe.X + exit.X, currentPipe.Y + exit.Y);
                lastExit = exit;
            }

            //Part 1
            var farthestPipe = Math.Ceiling((loop.Count-1)/2f);
            Console.WriteLine(farthestPipe);

            //Part 2
            var insideSpaces = 0;
            var loopByRows = loop.GroupBy(p => p.Y);
            foreach (var row in loopByRows)
            {
                var pipes = row.OrderBy(p => p.X).ToArray();
                var borders = new List<(Point Start, Point End, bool IsLine)>();
                for (int i = 0; i < pipes.Length; i++)
                {
                    var startPipe = pipes[i];
                    var endPipe = pipes[i];
                    var symbol = pipeland[startPipe.Y][startPipe.X];
                    var isStartUp = symbol == 'L';
                    var isEndUp = false;
                    var isLine = !isStartUp && symbol != 'F';
                    if (!isLine)
                    {
                        do
                        {
                            i++;
                            endPipe = pipes[i];
                            symbol = pipeland[endPipe.Y][endPipe.X];
                            isEndUp = symbol == 'J';
                        } while (!isEndUp && symbol != '7');
                    }
                    borders.Add((startPipe,endPipe, isLine || isStartUp != isEndUp));
                }

                for (int i = 0; i < borders.Count-1; i++)
                {
                    var current = borders[i];
                    var next = borders[i+1];
                    var skip = 1;
                    if (!current.IsLine)
                        continue;
                    while (!next.IsLine)
                    {
                        skip += next.End.X - next.Start.X + 1;
                        i++;
                        next = borders[i+1];
                    }
                    insideSpaces += next.Start.X - current.End.X - skip;
                    i++;
                }
            }
            Console.WriteLine(insideSpaces);
        }

        static string input = @""; //paste it manually from the page
    }
}
