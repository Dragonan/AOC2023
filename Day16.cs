using System.Drawing;

namespace AOC2023
{
    public static class Day16
    {
        static Dictionary<Point, List<Directions>> EnergizedTiles = new Dictionary<Point, List<Directions>>();
        static char[][] Contraption;
        public static void Solve()
        {
            Contraption = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();
            var max = 0;

            for (int i = 0; i < Contraption.Length; i++)
            {
                var entrances = new (Directions dir, Point pnt)[] {
                    (Directions.Right, new (-1, i)),
                    (Directions.Left, new (Contraption.Length, i)),
                    (Directions.Down, new (i, -1)),
                    (Directions.Up, new (i, Contraption.Length))
                };

                foreach (var entrance in entrances)
                {
                    MoveBeam(entrance.dir, entrance.pnt);
                    max = Math.Max(max, EnergizedTiles.Count);
                    EnergizedTiles.Clear();
                }
            }
            Console.WriteLine(max);
        }

        public static void SolvePart1()
        {
            Contraption = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();
            MoveBeam(Directions.Right, new (-1, 0));
            Console.WriteLine(EnergizedTiles.Count);
        }

        static void MoveBeam(Directions direction, Point coords)
        {
            char mirror;
            do
            {
                switch (direction)
                {
                    case Directions.Up: coords.Y--; break;
                    case Directions.Down: coords.Y++; break;
                    case Directions.Left: coords.X--; break;
                    case Directions.Right: coords.X++; break;
                    default: break;
                }
                
                if (coords.IsOutOfBounds(Contraption.Length))
                    return;
                if (EnergizedTiles.CheckForDuplicatesAndAdd(coords, direction))
                    return;

                mirror = Contraption[coords.Y][coords.X];
            } while (mirror == '.');

            if ((direction == Directions.Up && mirror == '|') ||
                (direction == Directions.Left && (mirror == '\\' || mirror == '|')) ||
                (direction == Directions.Right && (mirror == '/' || mirror == '|')))
            {
                MoveBeam(Directions.Up, coords);
            }

            if ((direction == Directions.Down && mirror == '|') ||
                (direction == Directions.Left && (mirror == '/' || mirror == '|')) ||
                (direction == Directions.Right && (mirror == '\\' || mirror == '|')))
            {
                MoveBeam(Directions.Down, coords);
            }

            if ((direction == Directions.Up && (mirror == '\\' || mirror == '-')) ||
                (direction == Directions.Down && (mirror == '/' || mirror == '-')) ||
                (direction == Directions.Left && mirror == '-'))
            {
                MoveBeam(Directions.Left, coords);
            }

            if ((direction == Directions.Up && (mirror == '/' || mirror == '-')) ||
                (direction == Directions.Down && (mirror == '\\' || mirror == '-')) ||
                (direction == Directions.Right && mirror == '-'))
            {
                MoveBeam(Directions.Right, coords);
            }
        }

        static string input = @""; //paste it manually from the page
    }
}