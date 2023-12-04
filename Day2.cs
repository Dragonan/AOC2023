namespace AOC2023
{
    public class Day2
    {
        public static void Solve()
        {
            var lines = input.Split('\n');
            var sum = 0;

            foreach (var line in lines)
            {
                sum += GetGamePower(line);
            }
            
            Console.WriteLine(sum);
        }

        private static int GetGameID(string line)
        {
            var data = line.Split(':');
            var id = int.Parse(data[0].Substring(5));

            var hints = data[1].Split(';');
            foreach (var hint in hints)
            {
                var cubes = hint.Split(',');
                foreach(var cube in cubes)
                {
                    var cubeData = cube.Trim().Split(' ');
                    var value = int.Parse(cubeData[0]);
                    var color = cubeData[1];
                    var max = int.MaxValue;
                    switch (color)
                    {
                        case "red": max = 12; break;
                        case "green": max = 13; break;
                        case "blue": max = 14; break;
                        default: break;
                    }
                    
                    if (max < value)
                        return 0;
                }
            }

            return id;
        }

        private static int GetGamePower(string line)
        {
            (int R, int G, int B) game = (0,0,0);

            var data = line.Split(':');
            var hints = data[1].Split(';');
            foreach (var hint in hints)
            {
                var cubes = hint.Split(',');
                foreach(var cube in cubes)
                {
                    var cubeData = cube.Trim().Split(' ');
                    var value = int.Parse(cubeData[0]);
                    var color = cubeData[1];
                    
                    switch (color)
                    {
                        case "red": game.R = int.Max(game.R, value); break;
                        case "green": game.G = int.Max(game.G, value); break;
                        case "blue": game.B = int.Max(game.B, value); break;
                        default: break;
                    }
                }
            }

            return game.R * game.G * game.B;
        }


        static string input = @""; //paste it manually from the page
    }
}
