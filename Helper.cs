using System.Drawing;

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

        public static bool IsOutOfBounds(this Point point, int lengthX, int lengthY = 0)
        {
            if (lengthY == 0)
                lengthY = lengthX;

            return point.X >= lengthX || 0 > point.X ||
                point.Y >= lengthY || 0 > point.Y;
        }

        public static bool CheckForDuplicatesAndAdd<TKey,T>(this Dictionary<TKey,List<T>> dictionary, TKey key, T value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<T> { value });
                return false;
            }
            
            if (!dictionary[key].Contains(value))
            {
                dictionary[key].Add(value);
                return false;
            }

            return true;
        }

        public static string ConvertToLetterName(this int number)
        {
            string columnName = "";

            while (number > 0)
            {
                int modulo = (number - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                number = (number - modulo) / 26;
            } 

            return columnName;
        }
    }
}
