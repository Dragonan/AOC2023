namespace AOC2023
{
    public static class Day12
    {
        public static void Solve()
        {
            var rows = input.Split('\n');
            var sum = 0;
            foreach (var row in rows)
            {
                var data = rows[15].Split(' ');
                var springs = data[0];
                var groups = data[1].Split(',').Select(g => int.Parse(g)).ToArray();
                
                sum += GetPossibleArrangements(springs, groups);
            }

            Console.WriteLine(sum);
        }

        static int GetPossibleArrangements(string springs, int[] groups)
        {
            var result = 0;
            springs = springs.TrimStart('.');
            var noMoreGroups = groups.Length == 0;
            var noMoreSprings = springs.Length == 0;
            
            if (noMoreSprings)
                return noMoreGroups ? 1 : 0;

            if (noMoreGroups)
                return springs.Contains('#') ? 0 : 1;
            
            if (springs.Length < groups[0])
                return 0;

            if (springs.StartsWith('#'))
            {
                var nextIndex = springs.IndexOfAny(new [] {'.','?'});
                var brokenLength = nextIndex < 0 ? springs.Length : nextIndex;
                if (brokenLength == groups[0])
                {
                    springs = nextIndex < 0 ? "" : '.' + springs.Substring(nextIndex + 1);
                    return GetPossibleArrangements(springs, groups.Skip(1).ToArray());
                }
                else if (brokenLength < groups[0])
                {
                    if (nextIndex < 0 || springs[nextIndex] == '.')
                        return 0;
                    
                    springs = springs.ReplaceAt(nextIndex,'#');
                    return GetPossibleArrangements(springs, groups);
                }
                else if (brokenLength > groups[0])
                    return 0;
            }

            var leftover = springs.Substring(1);
            result += GetPossibleArrangements('#' + leftover, groups);
            result += GetPossibleArrangements('.' + leftover, groups);

            return result;
        }

        static string input = @""; //paste it manually from the page
    }
}
