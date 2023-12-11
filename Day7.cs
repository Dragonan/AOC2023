namespace AOC2023
{
    public static class Day7
    {
        static int FiveOfAKind = 6;
        static int FourOfAKind = 5;
        static int FullHouse = 4;
        static int ThreeOfAKind = 3;
        static int TwoPairs = 2;
        static int OnePair = 1;
        static int AllDifferent = 0;

        public static void Solve()
        {
            
            var lines = input.Split('\n');
            var hands = new [] 
            {
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>(),
                new List<(string Hand, int Bid)>()
            };

            foreach (var line in lines)
            {
                var data = line.Trim().Split(' ');
                var hand = data[0];
                var bid = int.Parse(data[1]);
                var handCategory = GetHandCategoryPart1(ref hand);
                hands[handCategory].Add((hand, bid));
            }

            var handsSorted = 1;
            var sum = 0;

            for (int i = 0; i < hands.Length; i++)
            {
                hands[i] = hands[i].OrderBy(h => h.Hand).ToList();
                hands[i].ForEach(h => sum += h.Bid * handsSorted++);
            }

            Console.WriteLine(sum);
        }

        public static int GetHandCategoryPart1(ref string hand)
        {
            var kinds = hand.GroupBy(c => c).OrderByDescending(g => g.Count());
            var handCategory = AllDifferent;
            switch(kinds.Count())
            {
                case 1: handCategory = FiveOfAKind; break;
                case 2: handCategory = kinds.First().Count() == 4 ? FourOfAKind : FullHouse; break;
                case 3: handCategory = kinds.First().Count() == 3 ? ThreeOfAKind : TwoPairs; break;
                case 4: handCategory = OnePair; break;
                case 5: default: break;
            }
            
            hand = hand.Replace('A','E').Replace('T','A').Replace('J','B').Replace('Q','C').Replace('K','D');

            return handCategory;
        }


        public static int GetHandCategoryPart2(ref string hand)
        {
            //change the name of the cards so the standard string sorting orders them by power
            hand = hand.Replace('A','E').Replace('T','A').Replace('J','1').Replace('Q','C').Replace('K','D');
            var kinds = hand.GroupBy(c => c).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key);
            var handCategory = AllDifferent;
            var differentCards = kinds.Count();
            //1s (Js) are Jokers
            var strongestCombo = kinds.FirstOrDefault(g => g.Key != '1')?.Count();
            var jokers = hand.Count(c => c == '1');
            if (jokers > 0)
            {
                differentCards--;
                strongestCombo += jokers;
            }

            switch(differentCards)
            {
                //case 0 is in case of 11111
                case 0: case 1: handCategory = FiveOfAKind; break;
                case 2: handCategory = strongestCombo == 4 ? FourOfAKind : FullHouse; break;
                case 3: handCategory = strongestCombo == 3 ? ThreeOfAKind : TwoPairs; break;
                case 4: handCategory = OnePair; break;
                case 5: default: break;
            }
            
            return handCategory;
        }


        static string input = @""; //paste it manually from the page
    }
}
