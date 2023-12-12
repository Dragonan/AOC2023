namespace AOC2023
{
    public static class Day9
    {
        public static void Solve()
        {
            var sequences = input.Split('\n').Select(l => l.Split(' ').Select(n => int.Parse(n)).ToArray());
            var sum = 0;

            foreach (var sequence in sequences)
                sum += GetPrevValue(sequence);

            Console.WriteLine(sum);
        }

        static int GetNextValue(int[] sequence)
        {
            var subSequence = sequence;
            var nextValue = 0;

            while (subSequence.Any(v => v != 0))
            {
                nextValue += subSequence.Last();
                var newSequence = new int[subSequence.Length-1];

                for (int i = 0; i < newSequence.Length; i++)
                    newSequence[i] = subSequence[i+1] - subSequence[i];
                
                subSequence = newSequence;
            }

            return nextValue;
        }

        static int GetPrevValue(int[] sequence)
        {
            var subSequence = sequence;
            var firstValues = new List<int>();

            while (subSequence.Any(v => v != 0))
            {
                firstValues.Add(subSequence.First());
                var newSequence = new int[subSequence.Length-1];

                for (int i = 0; i < newSequence.Length; i++)
                    newSequence[i] = subSequence[i+1] - subSequence[i];
                
                subSequence = newSequence;
            }

            var prevValue = 0;
            for (int i = firstValues.Count-1; i >= 0; i--)
                prevValue = firstValues[i] - prevValue;

            return prevValue;
        }

        static string input = @""; //paste it manually from the page
    }
}
