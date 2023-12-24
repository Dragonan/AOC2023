using System.Text;

namespace AOC2023
{
    public class Lens
    {
        public string Label { get; set; }
        public byte FocalLength { get; set; }

        public Lens(string label, byte focalLength)
        {
            Label = label;
            FocalLength = focalLength;
        }
    }

    public static class Day15
    {
        public static void Solve()
        {
            SolvePart2();
        }

        static void SolvePart1()
        {
            var steps = input.Split(',');
            var sum = steps.Select(Hash).Sum();
            Console.WriteLine(sum);
        }

        static void SolvePart2()
        {
            var steps = input.Split(',');
            var boxes = new List<Lens>[256];
            boxes.FillWithEmptyLists();

            foreach (var step in steps)
            {
                if (step.EndsWith('-'))
                {
                    var label = step.Substring(0, step.Length - 1);
                    var i = Hash(label);
                    boxes[i].RemoveFirst(l => l.Label == label);
                }
                else
                {
                    var label = step.Substring(0, step.Length - 2);
                    var i = Hash(label);
                    var focalLength = byte.Parse(step.Last().ToString());
                    var oldLens = boxes[i].FirstOrDefault(l => l.Label == label);
                    if (oldLens != default)
                        oldLens.FocalLength = focalLength;
                    else
                        boxes[i].Add(new (label, focalLength));
                }
            }
            var sum = boxes.Select((b,i) => b.Select((l, j) => (i+1)*(j+1)*l.FocalLength).Sum()).Sum();
            Console.WriteLine(sum);
        }

        static int Hash(string text)
        {
            var ASCIIs = Encoding.ASCII.GetBytes(text);
            
            var value = 0;
            foreach (var ascii in ASCIIs)
            {
                value += ascii;
                value *= 17;
                value %= 256;
            }

            return value;
        }

        static string input = @""; //paste it manually from the page
    }
}