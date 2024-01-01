namespace AOC2023
{
    public static class ExtraMath
    {
        public static long FindLCM(long[] numbers)
        {
            var lcm = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
                lcm = lcm * numbers[i] / FindGCD(lcm, numbers[i]);
            return lcm;
        }

        public static long FindGCD(long a, long b)
        {
            if (b == 0)
                return a;
            return FindGCD(b, a % b);
        }

        public static long SumNNaturalNumbers(long n)
        {
            return n*(n+1)/2;
        }

        public static long SumAllOddNumbers(long max)
        {
            if (max % 2 == 1)
                max++;

            var n = max/2;

            return (long)Math.Pow(n,2);

        }

        public static long SumAllEvenNumbers(long max)
        {
            if (max % 2 == 1)
                max--;
            
            var n = max/2;
            return n*(n+1);
        }
    }
}