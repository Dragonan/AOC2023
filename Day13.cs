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
                var patterns = zone.Split('\n');
                var firstRowMirrors = new List<(int Index, bool HasSmudge)>();
                for (int i = 0; i < patterns[0].Length - 1; i++)
                {
                    var mirrored = IsMirroredAt(patterns[0], i);
                    if (mirrored.IsMirrored)
                        firstRowMirrors.Add((i,mirrored.HasSmudge));
                }

                for (int i = 1; i < patterns.Length; i++)
                {
                    if (!firstRowMirrors.Any())
                        break;

                    for (int j = 0; j < firstRowMirrors.Count; j++)
                    {
                        var firstMirror = firstRowMirrors[j];
                        var mirrored = IsMirroredAt(patterns[i], firstMirror.Index);
                        
                        if (!mirrored.IsMirrored || (mirrored.HasSmudge && firstMirror.HasSmudge))
                        {
                            firstRowMirrors.RemoveAt(j);
                            j--;
                        }

                        if (mirrored.IsMirrored && mirrored.HasSmudge && !firstMirror.HasSmudge)
                        {
                            firstMirror.HasSmudge = true;
                            firstRowMirrors[j] = firstMirror;
                        }
                        
                    }
                }

                if (firstRowMirrors.Any(m => m.HasSmudge))
                {
                    sum += firstRowMirrors.First(m => m.HasSmudge).Index + 1;
                    continue;
                }

                for (int i = 0; i < patterns.Length - 1; i++)
                {
                    var mirrored = true;
                    var hasSmudge = false;
                    for (int a=i, b=i+1; a>=0 && b < patterns.Length; a--, b++)
                    {
                        for (int j = 0; j < patterns[i].Length; j++)
                        {
                            if (patterns[a][j] != patterns[b][j])
                            {
                                if (!hasSmudge)
                                {
                                    hasSmudge = true;
                                }
                                else
                                {
                                    mirrored = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (mirrored && hasSmudge)
                    {
                        sum += 100*(i+1);
                        break;
                    }
                }
            }

            Console.WriteLine(sum);
        }

        static (bool IsMirrored, bool HasSmudge) IsMirroredAt(string pattern, int i)
        {
            var lastIndex = pattern.Length - 1;

            var mirrored = true;
            var hasSmudge = false;
            for (int a=i, b=i+1; a >= 0 && b <= lastIndex; a--, b++)
            {
                if (pattern[a] != pattern[b])
                {
                    if (!hasSmudge)
                    {
                        hasSmudge = true;
                    }
                    else
                    {
                        mirrored = false;
                        break;
                    }
                }
            }

            return (mirrored, hasSmudge);
        }

        static string input = @""; //paste it manually from the page
    }
}
