namespace AOC2023
{
    public static class Day6
    {
        public static void SolvePart1()
        {
            var data = input.Split('\n');
            var times = data[0].Split(':')[1].Trim().Split(' ').Where(t => t.Length > 0).Select(t => int.Parse(t.Trim()));
            var distances = data[1].Split(':')[1].Trim().Split(' ').Where(d => d.Length > 0).Select(d => int.Parse(d.Trim()));
            var games = times.Zip(distances, (t,d) => (Time: t, Distance: d));
            
            var sum = 1;
            foreach (var game in games)
            {
                var wins = 0;

                for (int i = 1; i < game.Time; i++)
                    if (i * (game.Time - i) > game.Distance)
                        wins++;

                sum *= wins;
            }

            Console.WriteLine(sum);
        }

        public static void SolvePart2()
        {
            var data = input.Split('\n');
            var time = long.Parse(data[0].Split(':')[1].Replace(" ",""));
            var distance = long.Parse(data[1].Split(':')[1].Replace(" ",""));
            
            var lowestWin = FindWinBoundry(time, distance, false);
            var highestWin = FindWinBoundry(time, distance, true);

            Console.WriteLine(highestWin - lowestWin + 1);
        }

        static long FindWinBoundry(long time, long distance, bool upper = false)
        {
            var counter = 0;
            var i = time/2;
            var step = time/4 * (upper ? 1 : -1);
            var hasWon = false;
            var prevWin = false;
            var direction = false;
            while (Math.Abs(step) > 1 || prevWin == hasWon)
            {
                prevWin = hasWon;
                var prevDirection = direction;
                i += step;
                direction = !(hasWon = i * (time - i) > distance);
                
                if (prevDirection != direction)
                    step *= -1;
                if (Math.Abs(step) > 1)
                    step /= 2;
                counter++;
            }
            Console.WriteLine(counter);
            return i + (hasWon ? 0 : (upper ? -1 : 1));
        }

        static string input = @""; //paste it manually from the page
    }
}
