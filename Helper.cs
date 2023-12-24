namespace AOC2023
{
    public static class CustomExtensions
    {
        public static string RemoveRF(this string text)
        {
            var firstNL = text.IndexOf('\n');
            if (firstNL > 0 && text[text.IndexOf('\n') - 1] == '\r')
                return text.Replace("\r","");
            return text;
        }

        public static string ReplaceAt(this string text, int index, char replacement)
        {
            var result = text.Substring(0, index) + replacement + text.Substring(index + 1);
            return result;
        }

        public static void RemoveFirst<T>(this List<T> list, Predicate<T> match)
        {
            var index = list.FindIndex(match);
            if (index != -1)
                list.RemoveAt(index);
        }

        public static void FillWithEmptyLists<T>(this List<T>[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = new List<T>();
        }
    }
}
