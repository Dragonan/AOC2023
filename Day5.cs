namespace AOC2023
{
    public static class Day5
    {
        public static void Solve()
        {
            var maps = input.Split("\n\n");
            var seeds = maps[0].Split(':')[1].TrimStart().Split(' ').Select(s => long.Parse(s)).ToArray();
            var sources = new List<(long LowerBound, long UpperBound)>();
            for (int i = 0; i < seeds.Length; i+=2)
                sources.Add((seeds[i], seeds[i] + seeds[i+1] - 1));
            sources.SortAndMergeRanges();
            var destinations = new List<(long LowerBound, long UpperBound)>();
            var mapGroups = new List<(long DestLowerBound, long SourceLowerBound, long SourceUpperBound)>();

            for (int i = 1; i < maps.Length; i++)
            {
                var lines = maps[i].Split('\n').Skip(1);
                foreach (var line in lines)
                {
                    var values = line.Split(' ').Select(v => long.Parse(v)).ToArray();
                    mapGroups.Add((values[0], values[1], values[1] + values[2] - 1));
                }
                mapGroups = mapGroups.OrderBy(m => m.SourceLowerBound).ToList();
                for (int j = 0; j < sources.Count; j++)
                {
                    var source = sources[j];
                    var destination = (source.LowerBound, source.UpperBound);
                    
                    //remove all map groups that are below the source range, to speed up the checks
                    while(mapGroups.Any() && mapGroups[0].SourceUpperBound < source.LowerBound)
                        mapGroups.RemoveAt(0);

                    var map = mapGroups.FirstOrDefault(m => m.SourceLowerBound <= source.LowerBound && source.LowerBound <= m.SourceUpperBound);

                    //lower bound is not in the map
                    if (map == default)
                    {
                        map = mapGroups.FirstOrDefault(m => m.SourceLowerBound <= source.UpperBound && source.UpperBound <= m.SourceUpperBound);
                        //upper bound is not in the map too - all source numbers remains unchanged
                        if (map == default)
                        {
                            destinations.Add(destination);
                            continue;
                        }
                        //upper bound is in the map - add lower numbers unchanged, make upper numbers the begining of a new range
                        else
                        {
                            destinations.Add((source.LowerBound, map.SourceLowerBound - 1));
                            destination.LowerBound = map.DestLowerBound;
                        }
                    }
                    //lower bound is in the map
                    else
                    {
                        destination.LowerBound = map.DestLowerBound + source.LowerBound - map.SourceLowerBound;
                    }

                    //assign the new upper bound number
                    destination.UpperBound = map.DestLowerBound + long.Min(source.UpperBound, map.SourceUpperBound) - map.SourceLowerBound;
                    destinations.Add(destination);


                    //uppwer bound is not in the current map group, create new source range using leftover upper numbers
                    if (map.SourceUpperBound < source.UpperBound)
                    {
                        sources.Insert(j+1, (map.SourceUpperBound + 1,source.UpperBound));
                    };
                }
                sources.Clear();
                sources.AddRange(destinations);
                sources.SortAndMergeRanges();
                destinations.Clear();
                mapGroups.Clear();
            }

            Console.WriteLine(sources.First());
        }

        static void SortAndMergeRanges(this List<(long LowerBound, long UpperBound)> ranges)
        {
            var temp = ranges.OrderBy(s => s.LowerBound).ToList();
            ranges.Clear();
            for (int i = 0; i < temp.Count; i++)
            {
                var lowerBound = temp[i].LowerBound;
                var upperBound = temp[i].UpperBound;
                while (i + 1 < temp.Count && upperBound + 1 == temp[i + 1].LowerBound)
                {
                    upperBound = temp[i + 1].UpperBound;
                    i++;
                }
                ranges.Add((lowerBound, upperBound));
            }
        }

        static string input = @""; //paste it manually from the page
    }
}
