namespace AOC2023
{
    static class Day4
    {
        public static void Solve()
        {
            var scratchcards = input.Split('\n');
            var sum = 0;
            var copies = Enumerable.Repeat(1, scratchcards.Length).ToArray();
            for (int i = 0; i < scratchcards.Length; i++)
            {
                var allNumbers = scratchcards[i].Split(':')[1].Split('|');
                var winningNumbers = allNumbers[0].Trim().Split(' ').Where(n => n.Length > 0);
                var myNumbers = allNumbers[1].Trim().Split(' ').Where(n => n.Length > 0);
                var matches = myNumbers.Count(n => winningNumbers.Contains(n));
                
                // if (matches > 0)
                //     sum += (int)Math.Pow(2,matches-1);

                for (int j = 0; j < matches; j++)
                {
                    copies[i+j+1] += 1*copies[i];
                }
            }

            sum = copies.Sum();

            Console.WriteLine(sum);
        }

        static string input = @""; //paste it manually from the page
    }
}
