namespace AOC2023
{
    public static class Graphics
    {
        public static void Draw(IEnumerable<string> lines)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            foreach (var line in lines)
                Console.WriteLine(line);
        }
    }
}