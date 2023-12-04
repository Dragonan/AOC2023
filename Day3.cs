namespace AOC2023
{
    public static class Day3
    {
        static int engineMaxRowIndex;
        static int engineMaxColIndex;

        public static void Solve()
        {
            var engine = input.Split('\n');
            var sum = 0;
            engineMaxRowIndex = engine.Length - 1;
            engineMaxColIndex = engine[1].TrimEnd().Length - 1;

            for (int i = 0; i < engine.Length; i++)
            {
                engine[i] = engine[i].TrimEnd();
                for (int j = 0; j < engine[i].Length; j++)
                {
                    sum += GetGearRatio(engine, i, j);
                }
            }

            Console.WriteLine(sum);
        }

        static int GetGearRatio(string[] engine, int i, int j)
        {
            var row = engine[i];
            if (row[j] == '*')
            {
                var top = int.Max(i-1, 0);
                var bottom = int.Min(i+1, engineMaxRowIndex);
                var left = int.Max(j-1, 0);
                var right = int.Min(j+1, engineMaxColIndex);
                
                var numbers = new List<int>();

                for (int k = top; k <= bottom; k++)
                {
                    for (int l = left; l <= right; l++)
                    {
                        var num = GetEnginePart(engine, k, ref l, true);
                        if (num != 0)
                            numbers.Add(num);
                    }
                }

                if (numbers.Count == 2)
                    return numbers[0] * numbers[1];
            }

            return 0;
        }

        static int GetEnginePart(string[] engine, int i, ref int j, bool skipCheck = false)
        {
            var row = engine[i];

            if (char.IsDigit(row[j]))
            {
                var left = int.Max(j-1, 0);
                var right = int.Min(j+1, engineMaxRowIndex);
                var top = int.Max(i-1, 0);
                var bottom = int.Min(i+1, engineMaxColIndex);
                var hasAdjacentSymbol = false;

                if (!skipCheck)
                {
                    var adjacentChars = new [] {
                        engine[top][left],    engine[top][j],    engine[top][right],
                        engine[i][left],                         engine[i][right],
                        engine[bottom][left], engine[bottom][j], engine[bottom][right]
                    };
                    foreach (var adjChar in adjacentChars)
                    {
                        if (!char.IsDigit(adjChar) && adjChar != '.')
                        {
                            hasAdjacentSymbol = true;
                            break;
                        }
                    }
                }

                if (skipCheck || hasAdjacentSymbol)
                {
                    var numberStr = "";
                    if (j > 0)
                    {
                        for (var k = left; k >= 0; k--)
                        {
                            if (char.IsDigit(row[k]))
                                numberStr = row[k] + numberStr;
                            else
                                break;
                        }
                    }
                    numberStr += row[j];
                    if (j < engineMaxColIndex)
                    {
                        for (var k = right; k <= engineMaxColIndex; k++)
                        {
                            if (char.IsDigit(row[k]))
                            {
                                numberStr += row[k];
                                j++;
                            }
                            else
                                break;
                        }
                    }
                    return int.Parse(numberStr);
                }
            }

            return 0;
        }



        static string input = @""; //paste it manually from the page
    }
}
