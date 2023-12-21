namespace AOC2023
{
    public static class Day12
    {
        public static void Solve()
        {
            var rows = input.Split('\n');
            var sum = 0L;
            foreach (var row in rows)
            {
                var data = row.Split(' ');
                var springs = data[0];
                var groups = data[1].Split(',').Select(int.Parse).ToArray();

                sum += GetMultipleArrangements(springs, groups, 5);
            }

            Console.WriteLine(sum);
        }

        static long GetMultipleArrangements(string springs, int[] groups, int repeat, long multiplier = 1, 
                                            string? originalSprings = null, int[]? originalGroups = null)
        {
            var result = 0L;
            originalGroups = originalGroups ?? groups;
            originalSprings = originalSprings ?? springs;
            if (repeat == 0)
            {
                if (groups.Length > originalGroups.Length)
                    return 0;
                return multiplier;
            }
            
            var leftovers = GetPossibleArrangements(springs, groups);
            leftovers.MultiplyCounts(multiplier);

            foreach (var leftover in leftovers)
            {
                if (leftover.Key.StartsWith('G'))
                {
                    var parts = leftover.Key.Split('|');
                    var leftoverGroupsCount = int.Parse(parts[0].Substring(1));
                    if (leftoverGroupsCount == 0 && repeat == 1)
                        continue;

                    var oldGroups = originalGroups;
                    while (leftoverGroupsCount > originalGroups.Length)
                    {
                        leftoverGroupsCount -= originalGroups.Length;
                        oldGroups = oldGroups.Concat(originalGroups).ToArray();
                    }
                    var totalGroups = originalGroups.Skip(originalGroups.Length - leftoverGroupsCount).Concat(oldGroups).ToArray();
                    var newLeftover = parts[1];
                    var springsWithLeftover = (newLeftover == "-" ? "" : newLeftover + '?') + originalSprings;
                    result += GetMultipleArrangements(springsWithLeftover, totalGroups, repeat-1, leftover.Value, originalSprings, originalGroups);
                }
                else
                {
                    var springsWithLeftover = (leftover.Key == "-" ? "" : leftover.Key + '?') + originalSprings;
                    result += GetMultipleArrangements(springsWithLeftover, originalGroups, repeat-1, leftover.Value, originalSprings, originalGroups);
                }
            }
            
            return result;
        }

        static Dictionary<string, long> GetPossibleArrangements(string springs, int[] groups)
        {
            var result = new Dictionary<string, long>();
            var realSprings = springs;
            springs = springs.TrimStart('.');
            var noMoreGroups = groups.Length == 0;
            var noMoreSprings = springs.Length == 0;
            
            if (noMoreSprings)
            {
                var endLeftover = realSprings.Any() ? "" : "-";
                if (noMoreGroups)
                    result.Add(endLeftover, 1);
                else
                    result.Add($"G{groups.Length}|{endLeftover}", 1);
                return result;
            }

            if (noMoreGroups)
            {
                if (!springs.Contains('#')) 
                    result.Add(springs, 1);
                else
                    result.Add($"G0|{springs}", 1);
                return result;
            }
            
            if (springs.Length < groups[0])
            {
                result.Add($"G{groups.Length}|{springs}", 1);
                return result;
            }

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
                    {
                        result.Add($"G{groups.Length}|{springs}", 1);
                        return result;
                    }
                    
                    springs = springs.ReplaceAt(nextIndex,'#');
                    return GetPossibleArrangements(springs, groups);
                }
                else if (brokenLength > groups[0])
                {
                    result.Add($"G{groups.Length}|{springs}", 1);
                    return result;
                }
            }

            var leftover = springs.Substring(1);
            result.CombineCounts(GetPossibleArrangements('#' + leftover, groups));
            result.CombineCounts(GetPossibleArrangements('.' + leftover, groups));

            return result;
        }

        public static void MultiplyCounts(this Dictionary<string, long> leftovers, long multiplier)
        {
            if (multiplier == 1)
                return;
            
            foreach (var leftover in leftovers.Keys)
            {
                leftovers[leftover] *= multiplier;
            }
        }

        public static void CombineCounts(this Dictionary<string, long> leftovers, Dictionary<string, long> newLeftovers)
        {
            foreach (var leftover in newLeftovers)
            {
                if (leftovers.ContainsKey(leftover.Key))
                    leftovers[leftover.Key] += leftover.Value;
                else
                    leftovers[leftover.Key] = leftover.Value;
            }
        }

        static string input = @""; //paste it manually from the page
    }
}
