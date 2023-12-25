using System.Drawing;

namespace AOC2023
{
    public struct DirectionWithSpeed : IEquatable<DirectionWithSpeed>
    {
        public DirectionWithSpeed(Directions dir, byte speed)
        {
            Direction = dir;
            Speed = speed;
        }

        public DirectionWithSpeed(int dir, byte speed)
        {
            Direction = (Directions)dir;
            Speed = speed;
        }

        public Directions Direction { get; set; }
        public byte Speed { get; set; }

        public bool Equals(DirectionWithSpeed other)
        {
            return this.Direction == other.Direction && this.Speed == other.Speed;
        }

        public static bool operator ==(DirectionWithSpeed ds1, DirectionWithSpeed ds2)
        {
            return ds1.Equals(ds2);
        }

        public static bool operator !=(DirectionWithSpeed ds1, DirectionWithSpeed ds2)
        {
            return !ds1.Equals(ds2);
        }

        public override string ToString()
        {
            return Direction + ", " + Speed;
        }
    }

    public static class Day17
    {
        static byte[][] Blocks;
        static Dictionary<DirectionWithSpeed, int>[,] HeatLossMap;
        static bool[,] VisitedBlocks;
        static List<Point> BlocksToUpdate = new List<Point>();
        public static void Solve()
        {
            Blocks = input.Split('\n').Select(r => r.TrimEnd().Select(c => byte.Parse(c.ToString())).ToArray()).ToArray();
            HeatLossMap = new Dictionary<DirectionWithSpeed, int>[Blocks.Length,Blocks.Length];
            for (int i = 0; i < Blocks.Length; i++)
                for (int j = 0; j < Blocks.Length; j++)
                    HeatLossMap[i,j] = new Dictionary<DirectionWithSpeed, int>();
            VisitedBlocks = new bool[Blocks.Length,Blocks.Length];

            FillHeatLossMap();

            var lowestHeatLoss = HeatLossMap[Blocks.Length-1,Blocks.Length-1].Values.Min();
            Console.WriteLine(lowestHeatLoss);
        }

        static void FillHeatLossMap()
        {
            HeatLossMap[0,0] = new Dictionary<DirectionWithSpeed, int> {
                { new (Directions.Down, 1), 0 },
                { new (Directions.Right, 1), 0 }
            };

            for (int i = 0; i < Blocks.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    UpdateNode(i, j);
                    UpdateNode(j, i);
                }
                
                UpdateNode(i, i);

                while(BlocksToUpdate.Any())
                {
                    var toUpdate = BlocksToUpdate.ToArray();
                    BlocksToUpdate.Clear();

                    foreach (var block in toUpdate)
                        UpdateNode(block.X, block.Y);
                }
            }
        }

        static void UpdateNode(int x, int y)
        {
            var currentNode = HeatLossMap[y, x];
            if (x > 0) 
                UpdateNodeForPart2(Directions.Left, new (x-1,y), Directions.Right, currentNode);
            if (x < Blocks.Length - 1) 
                UpdateNodeForPart2(Directions.Right, new (x+1,y), Directions.Left, currentNode);
            if (y > 0) 
                UpdateNodeForPart2(Directions.Up, new (x,y-1), Directions.Down, currentNode);
            if (y < Blocks.Length - 1) 
                UpdateNodeForPart2(Directions.Down, new (x,y+1), Directions.Up, currentNode);

            VisitedBlocks[y,x] = true;
        }

        static void UpdateNodeForPart1(Directions dir, Point coords, Directions oppositeDir, Dictionary<DirectionWithSpeed, int> currentNode)
        {
            var heatLossesFor1Speed = currentNode.Where(k => k.Key.Direction != dir && k.Key.Direction != oppositeDir).Select(n => n.Value).ToArray();
            var keyFor2Speed = new DirectionWithSpeed(dir, 1);
            var keyFor3Speed = new DirectionWithSpeed(dir, 2);
            var heatLosses = new int[3];
            heatLosses[0] = heatLossesFor1Speed.Any() ? heatLossesFor1Speed.Min() : -1;
            heatLosses[1] = currentNode.ContainsKey(keyFor2Speed) ? currentNode[keyFor2Speed] : -1;
            heatLosses[2] = currentNode.ContainsKey(keyFor3Speed) ? currentNode[keyFor3Speed] : -1;

            var adjNode = HeatLossMap[coords.Y, coords.X];
            var heatLoss = Blocks[coords.Y][coords.X];
            var dirWithSpeed = new DirectionWithSpeed(dir, 0);
            
            for (int i = 0; i < heatLosses.Length; i++)
            {
                dirWithSpeed.Speed++;
                if (heatLosses[i] < 0)
                    continue;
                var newHeatLoss = heatLosses[i] + heatLoss;
                if (!adjNode.ContainsKey(dirWithSpeed) || adjNode[dirWithSpeed] > newHeatLoss)
                {
                    adjNode[dirWithSpeed] = newHeatLoss;
                    if (VisitedBlocks[coords.Y,coords.X])
                        BlocksToUpdate.Add(coords);
                }
            }
        }
    
        static void UpdateNodeForPart2(Directions dir, Point coords, Directions oppositeDir, Dictionary<DirectionWithSpeed, int> currentNode)
        {
            var distanceToEnd = 3;
            switch (dir)
            {
                case Directions.Up: distanceToEnd = coords.Y; break;
                case Directions.Down: distanceToEnd = Blocks.Length - 1 - coords.Y; break;
                case Directions.Left: distanceToEnd = coords.X; break;
                case Directions.Right: distanceToEnd = Blocks.Length - 1 - coords.X; break;
                default: break;
            }

            var heatLossesFromTurning = distanceToEnd >= 3 
                ? currentNode
                    .Where(k => k.Key.Direction != dir && k.Key.Direction != oppositeDir && k.Key.Speed >= 4)
                    .Select(n => n.Value).ToArray() 
                : new int[0];
            var keysForGoingStraight = new DirectionWithSpeed[] { 
                new (dir, 1), new (dir, 2), new (dir, 3),
                new (dir, 4), new (dir, 5), new (dir, 6),
                new (dir, 7), new (dir, 8), new (dir, 9)
            };
            var heatLosses = new int[10];
            heatLosses[0] = heatLossesFromTurning.Any() ? heatLossesFromTurning.Min() : -1;
            heatLosses[1] = distanceToEnd >= 2 && currentNode.ContainsKey(keysForGoingStraight[0]) ? currentNode[keysForGoingStraight[0]] : -1;
            heatLosses[2] = distanceToEnd >= 1 && currentNode.ContainsKey(keysForGoingStraight[1]) ? currentNode[keysForGoingStraight[1]] : -1;
            for (int i = 2; i < keysForGoingStraight.Length; i++)
                heatLosses[i+1] = currentNode.ContainsKey(keysForGoingStraight[i]) ? currentNode[keysForGoingStraight[i]] : -1;

            var adjNode = HeatLossMap[coords.Y, coords.X];
            var heatLoss = Blocks[coords.Y][coords.X];
            var dirWithSpeed = new DirectionWithSpeed(dir, 0);
            var updatedLowestHeat = false;
            
            for (int i = 0; i < heatLosses.Length; i++)
            {
                dirWithSpeed.Speed++;
                if (heatLosses[i] < 0)
                    continue;
                var newHeatLoss = heatLosses[i] + heatLoss;
                if (!adjNode.ContainsKey(dirWithSpeed) || adjNode[dirWithSpeed] > newHeatLoss)
                {
                    adjNode[dirWithSpeed] = newHeatLoss;
                    if (VisitedBlocks[coords.Y,coords.X])
                        updatedLowestHeat = true;
                }
            }

            if (updatedLowestHeat)
                BlocksToUpdate.Add(coords);
        }

        static string input = @""; //paste it manually from the page
    }
}