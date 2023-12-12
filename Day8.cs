using System.Reflection.Metadata.Ecma335;

namespace AOC2023
{
    class TreeNode
    {
        public TreeNode(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public static class Day8
    {
        public static void Solve()
        {
            input = input.RemoveRF();
            var splitIndex = input.IndexOf("\n\n");;
            var instructions = input.Substring(0, splitIndex);
            var lines = input.Substring(splitIndex+2).Split('\n');

            var steps = SolvePart2(instructions, lines);

            Console.WriteLine(steps);
        }

        static long SolvePart1(string instructions, string[] lines)
        {
            var nodes = new List<TreeNode> { new TreeNode("AAA")};
            var currentNode = nodes.First();

            foreach (var line in lines)
            {
                var data = line.Split('=');
                var name = data[0].Trim();
                var connections = data[1].Split(',');
                var left = connections[0].Substring(2);
                var right = connections[1].Substring(1,3);

                var newNode = nodes.FindOrAddNode(name);
                newNode.Left = nodes.FindOrAddNode(left);
                newNode.Right = nodes.FindOrAddNode(right);
                
                if (name == "AAA")
                    currentNode = newNode;
            }

            var steps = 0;
            for (int i = 0; i < instructions.Length; i++)
            {
                steps++;
                currentNode = instructions[i] == 'L' ? currentNode.Left : currentNode.Right;
            
                if (currentNode.Name == "ZZZ")
                    break;

                if (i == instructions.Length - 1)
                    i = -1;
            }

            return steps;
        }

        static long SolvePart2(string instructions, string[] lines)
        {
            var nodes = new List<TreeNode>();
            var startNodes = new List<TreeNode>();

            foreach (var line in lines)
            {
                var data = line.Split('=');
                var name = data[0].Trim();
                var connections = data[1].Split(',');
                var left = connections[0].Substring(2);
                var right = connections[1].Substring(1,3);

                var newNode = nodes.FindOrAddNode(name);
                newNode.Left = nodes.FindOrAddNode(left);
                newNode.Right = nodes.FindOrAddNode(right);
                
                if (name.EndsWith('A') && !startNodes.Contains(newNode))
                    startNodes.Add(newNode);
            }

            var routes = new int[startNodes.Count];
            for (int j = 0; j < startNodes.Count; j++)
            {
                var steps = 0;
                var currentNode = startNodes[j];
                for (int i = 0; i < instructions.Length; i++)
                {
                    steps++;
                    currentNode = instructions[i] == 'L' ? currentNode.Left : currentNode.Right;
                
                    if (currentNode.Name.EndsWith('Z'))
                    {
                        routes[j] = steps;
                        break;
                    }

                    if (i == instructions.Length - 1)
                        i = -1;
                }
            }

            return FindLCM(routes.Select(n => (long)n).ToArray());

        }


        static TreeNode FindOrAddNode(this List<TreeNode> nodes, string name)
        {
            var node = nodes.FirstOrDefault(n => n.Name == name);
            if (node == null)
            {
                node = new TreeNode(name);
                nodes.Add(node);
            }
            return node;
        }

        static long FindLCM(long[] numbers)
        {
            var lcm = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
                lcm = lcm * numbers[i] / FindGCD(lcm, numbers[i]);
            return lcm;
        }

        static long FindGCD(long a, long b)
        {
            if (b == 0)
                return a;
            return FindGCD(b, a % b);
        }



        static string input = @""; //paste it manually from the page
    }
}
