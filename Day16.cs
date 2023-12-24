using System.Drawing;

namespace AOC2023
{
    public enum Direcitons { Up, Right, Down, Left }

    public static class Day16
    {
        static Dictionary<Point, Direcitons> EnergizedTiles = new Dictionary<Point, Direcitons>();
        static char[][] Contraption;
        public static void Solve()
        {
            Contraption = input.Split('\n').Select(r => r.TrimEnd().ToCharArray()).ToArray();
            var max = 0;

            for (int i = 0; i < Contraption.Length; i++)
            {
                var entrances = new (Direcitons dir, Point pnt)[] {
                    (Direcitons.Right, new (-1, i)),
                    (Direcitons.Left, new (Contraption.Length, i)),
                    (Direcitons.Down, new (i, -1)),
                    (Direcitons.Up, new (i, Contraption.Length))
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
            MoveBeam(Direcitons.Right, new (-1, 0));
            Console.WriteLine(EnergizedTiles.Count);
        }

        static void MoveBeam(Direcitons direction, Point coords)
        {
            char mirror;
            do
            {
                switch (direction)
                {
                    case Direcitons.Up: coords.Y--; break;
                    case Direcitons.Down: coords.Y++; break;
                    case Direcitons.Left: coords.X--; break;
                    case Direcitons.Right: coords.X++; break;
                    default: break;
                }
                
                if (coords.IsOutOfBounds(Contraption.Length))
                    return;
                if (CheckForLoopAndAdd(coords, direction))
                    return;

                mirror = Contraption[coords.Y][coords.X];
            } while (mirror == '.');

            if ((direction == Direcitons.Up && mirror == '|') ||
                (direction == Direcitons.Left && (mirror == '\\' || mirror == '|')) ||
                (direction == Direcitons.Right && (mirror == '/' || mirror == '|')))
            {
                MoveBeam(Direcitons.Up, coords);
            }

            if ((direction == Direcitons.Down && mirror == '|') ||
                (direction == Direcitons.Left && (mirror == '/' || mirror == '|')) ||
                (direction == Direcitons.Right && (mirror == '\\' || mirror == '|')))
            {
                MoveBeam(Direcitons.Down, coords);
            }

            if ((direction == Direcitons.Up && (mirror == '\\' || mirror == '-')) ||
                (direction == Direcitons.Down && (mirror == '/' || mirror == '-')) ||
                (direction == Direcitons.Left && mirror == '-'))
            {
                MoveBeam(Direcitons.Left, coords);
            }

            if ((direction == Direcitons.Up && (mirror == '/' || mirror == '-')) ||
                (direction == Direcitons.Down && (mirror == '\\' || mirror == '-')) ||
                (direction == Direcitons.Right && mirror == '-'))
            {
                MoveBeam(Direcitons.Right, coords);
            }
        }

        static bool CheckForLoopAndAdd(Point coords, Direcitons direction)
        {
            if (!EnergizedTiles.ContainsKey(coords))
            {
                EnergizedTiles.Add(coords, direction);
                return false;
            }

            return EnergizedTiles[coords] == direction;
        }

        static string input = @""; //paste it manually from the page
    }
}