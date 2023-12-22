namespace AOC2023
{
    public static class Day13
    {
        public static void Solve()
        {
            var valley = input.RemoveRF().Split("\n\n");
            var sum = 0;

            foreach (var zone in valley)
            {
                var patternSum = 0;
                var patterns = zone.Split('\n');
                var firstRowMirrors = new List<int>();
                for (int i = 0; i < patterns[0].Length - 1; i++)
                {
                    if (IsMirroredAt(patterns[0], i))
                        firstRowMirrors.Add(i);
                }

                for (int i = 1; i < patterns.Length; i++)
                {
                    if (!firstRowMirrors.Any())
                        break;

                    for (int j = 0; j < firstRowMirrors.Count; j++)
                    {
                        if (!IsMirroredAt(patterns[i],firstRowMirrors[j]))
                        {
                            firstRowMirrors.RemoveAt(j);
                            j--;
                        }
                    }
                }

                if (firstRowMirrors.Any())
                {
                    patternSum += firstRowMirrors.First() + 1;
                    Console.WriteLine(patternSum);
                    sum += patternSum;
                    continue;
                }

                for (int i = 0; i < patterns.Length - 1; i++)
                {
                    var mirrored = true;
                    for (int a=i, b=i+1; a>=0 && b < patterns.Length; a--, b++)
                    {
                        if (patterns[a] != patterns[b])
                        {
                            mirrored = false;
                            break;
                        }
                    }

                    if (mirrored)
                    {
                        patternSum += 100*(i+1);
                        break;
                    }
                }

                Console.WriteLine(patternSum);
                sum += patternSum;
            }

            Console.WriteLine(sum);
        }

        static bool IsMirroredAt(string pattern, int i)
        {
            var lastIndex = pattern.Length - 1;
            if (i < 0 || i >= lastIndex)
                return false;

            var mirrored = true;
            for (int a=i, b=i+1; a >= 0 && b <= lastIndex; a--, b++)
            {
                if (pattern[a] != pattern[b])
                {
                    mirrored = false;
                    break;
                }
            }

            return mirrored;
        }

        static string input = @""; //paste it manually from the page
    }
}
