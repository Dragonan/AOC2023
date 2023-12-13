namespace AOC2023
{
    public static class Day11
    {
        public static void Solve()
        {
            var universe = input.RemoveRF().Split('\n').ToList();
            var collumnsWithGalaxies = new List<int>();
            var emptyRows = new List<int>();
            var galaxies = new List<(long X, long Y)>();
            for (int i = 0; i < universe.Count; i++)
            {
                var hasGalaxy = false;
                for (int j = 0; j < universe[i].Length; j++)
                {
                    if (universe[i][j] == '#')
                    {
                        hasGalaxy = true;
                        galaxies.Add((j, i));
                        if (!collumnsWithGalaxies.Contains(j))
                            collumnsWithGalaxies.Add(j);
                    }
                }
                if (!hasGalaxy)
                    emptyRows.Add(i);
            }

            //Part 1 = 1
            //Part 2 = 1000000 - 1
            var universeExpansion = 1000000 - 1;

            //expand horizontally
            for (int i = universe[0].Length - 1; i >= 0; i--)
                if (!collumnsWithGalaxies.Contains(i))
                    for (int j = 0; j < galaxies.Count; j++)
                        if (galaxies[j].X > i)
                            galaxies[j] = (X: galaxies[j].X + universeExpansion, Y: galaxies[j].Y);
            
            //expand vertically
            for (int i = emptyRows.Count - 1; i >= 0; i--)
                for (int j = galaxies.Count - 1; j >= 0; j--)
                    if (galaxies[j].Y > emptyRows[i])
                        galaxies[j] = (X: galaxies[j].X, Y: galaxies[j].Y + universeExpansion);

            var sum = 0L;
            for (int i = 0; i < galaxies.Count - 1; i++)
            {
                for (int j = i+1; j < galaxies.Count; j++)
                {
                    sum += Math.Abs(galaxies[i].X - galaxies[j].X) + Math.Abs(galaxies[i].Y - galaxies[j].Y);
                }
            }

            Console.WriteLine(sum);
        }

        static string input = @""; //paste it manually from the page
    }
}
