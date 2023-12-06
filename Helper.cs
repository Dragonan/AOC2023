namespace AOC2023
{
    public static class CustomExtensions
    {
        public static string RemoveRF(this string text)
        {
            var firstNL = text.IndexOf('\n');
            if (firstNL > 0 && text[text.IndexOf('\n') - 1] == '\r')
                return text.Remove('\r');
            return text;
        }
    }
}
