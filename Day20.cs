using System.Diagnostics;

namespace AOC2023
{
    public abstract class GateModule
    {
        protected const bool HighPulse = true;
        protected const bool LowPulse = false;
        public GateModule(string name)
        {
            this.Name = name;
            Destinations = new List<GateModule>();
        }

        public string Name { get; }
        public virtual bool Pulse { get; protected set; }
        public List<GateModule> Destinations { get; }
        public bool IsEnd { get; set; }
        public bool EndPulse { get; set; }
        
        public void SendPulse()
        {
            if (IsEnd && Pulse == EndPulse)
                ModuleBuffer.EndStateReached = true;
 
            foreach (var module in Destinations)
            {
                module.ReceivePulse(Pulse, Name);
                ModuleBuffer.PulsesCount[Pulse]++;
            }
        }

        public abstract void ReceivePulse(bool pulse, string from);
        public abstract void Reset();
    }

    public class FlipFlopModule : GateModule
    {
        public FlipFlopModule(string name) : base(name) { }

        public override bool Pulse { get => State; }
        public bool State { get; private set; }
        
        public override void ReceivePulse(bool pulse, string from)
        {
            if (pulse == LowPulse)
            {
                State = !State;
                ModuleBuffer.Modules.Add(this);
            }
        }

        public override void Reset()
        {
            this.State = LowPulse;
        }
    }

    public class ConjuctionModule : GateModule
    {
        public ConjuctionModule(string name) : base(name)
        {
            Inputs = new Dictionary<string, bool>();
        }

        public override bool Pulse { get => Inputs.Values.Any(p => p == LowPulse); }
        public Dictionary<string, bool> Inputs { get; }

        public override void ReceivePulse(bool pulse, string from)
        {
            Inputs[from] = pulse;
            ModuleBuffer.Modules.Add(this);
        }
        
        public override void Reset()
        {
            foreach (var key in Inputs.Keys)
                Inputs[key] = LowPulse;
        }
    }

    public class BroadcastModule : GateModule
    {
        public BroadcastModule() : base("broadcaster") { }

        public override void ReceivePulse(bool pulse, string from = "")
        {
            this.Pulse = pulse;
            ModuleBuffer.Modules.Add(this);
        }

        public override void Reset()
        {
            this.Pulse = LowPulse;
        }
    }

    public class SinkModule : GateModule
    {
        public SinkModule(string name) : base(name) 
        {
            IsEnd = true;
            EndPulse = LowPulse;
        }

        public override void ReceivePulse(bool pulse, string from = "")
        {
            Pulse = pulse;
            if (Pulse == EndPulse)
                ModuleBuffer.EndStateReached = true;
        }
        
        public override void Reset()
        {
            this.Pulse = LowPulse;
        }
    }


    public static class ModuleBuffer
    {
        public static List<GateModule> Modules = new List<GateModule>();
        public static Dictionary<bool, int> PulsesCount = new Dictionary<bool, int> 
            { { true, 0 }, { false, 0 } };

        public static bool EndStateReached = false;

        public static void Execute()
        {
            var innerBuffer = Modules.ToList();
            Modules.Clear();
            foreach (var module in innerBuffer)
                module.SendPulse();
        }
    }

    public static class Day20
    {
        const bool HighPulse = true;
        const bool LowPulse = false;
        static Dictionary<string, GateModule> Modules;
        public static void Solve()
        {
            var text = input.RemoveRF().Split('\n');
            ParseModules(text);
            SolvePart2();
        }

        static void SolvePart1()
        {
            var highPulsesCount = new List<int>();
            var lowPulsesCount = new List<int>();
            var originalStates = GetModuleStates();
            
            for (int i = 0; i < 1000; i++)
            {
                PressButton();
                highPulsesCount.Add(ModuleBuffer.PulsesCount[HighPulse]);
                lowPulsesCount.Add(ModuleBuffer.PulsesCount[LowPulse]);
                ModuleBuffer.PulsesCount[HighPulse] = 0;
                ModuleBuffer.PulsesCount[LowPulse] = 0;

                var currentStates = GetModuleStates();
                if (originalStates.SequenceEqual(currentStates))
                    break;
            }

            var cycleLength = highPulsesCount.Count;
            var highPulses = highPulsesCount.Sum() * Math.Floor(1000d/cycleLength);
            var lowPulses = lowPulsesCount.Sum() * Math.Floor(1000d/cycleLength);
            highPulses += highPulsesCount.Take(1000 % cycleLength).Sum();
            lowPulses += lowPulsesCount.Take(1000 % cycleLength).Sum();

            Console.WriteLine(highPulses * lowPulses);
        }

        static void SolvePart2()
        {
            var cycles = GetCyclesToReachEndModule();
            Console.WriteLine(cycles);
        }

        static void PressButton(string startModule = "broadcaster")
        {
            var broadcaster = Modules[startModule];
            ModuleBuffer.PulsesCount[LowPulse]++;
            broadcaster.ReceivePulse(LowPulse, string.Empty);
            while (ModuleBuffer.Modules.Any())
                ModuleBuffer.Execute();
        }

        static bool[] GetModuleStates()
        {
            var states = new List<bool>();
            foreach (var module in Modules.Values)
            {
                if (module.GetType() == typeof(FlipFlopModule))
                    states.Add(module.Pulse);
                else if (module.GetType() == typeof(ConjuctionModule))
                    states.AddRange(((ConjuctionModule)module).Inputs.Select(i => i.Value));
            }
            return states.ToArray();
        }

        static void ResetModules()
        {
            foreach (var module in Modules.Values)
            {
                module.Reset();
                if (module.GetType() != typeof(SinkModule))
                    module.IsEnd = false;
            }

            ModuleBuffer.Modules.Clear();
            ModuleBuffer.EndStateReached = false;
        }

        // The structure is the same for each input.
        // This can be changed to work recursively with all types of 
        // module structures, but there is no point to do that for 
        // this solution.
        //               startName                                                      endName
        //broadcaster -> flipflops -> (many flipflops), conjuction -> (many flipflops), conjunction -> conjuction -> end
        static long GetCyclesToReachEndModule()
        {
            var broadcaster = Modules["broadcaster"];
            var innerCycles = new List<(string startModule, string endModule)>();
            foreach (var dest in broadcaster.Destinations)
            {
                var endModule = dest
                    .Destinations.First(m => m.GetType() == typeof(ConjuctionModule))
                    .Destinations.First(m => m.GetType() == typeof(ConjuctionModule));
                
                innerCycles.Add((dest.Name, endModule.Name));
            }
            var cycleLenghts = Enumerable.Repeat(0L, innerCycles.Count).ToArray();

            for (int i = 0; i < innerCycles.Count; i++)
            {
                var endModule = Modules[innerCycles[i].endModule];
                endModule.IsEnd = true;
                endModule.EndPulse = HighPulse;
                while (!ModuleBuffer.EndStateReached)
                {
                    PressButton(innerCycles[i].startModule);
                    cycleLenghts[i]++;
                }
                ResetModules();
            }

            return ExtraMath.FindLCM(cycleLenghts);
        }

        static void ParseModules(string[] text)
        {
            Modules = new Dictionary<string, GateModule>();
            var destinationsInfo = new Dictionary<string, string[]>();
            foreach (var row in text)
            {
                var data = row.Split(" -> ");
                var name = data[0];
                var type = data[0][0];
                if (type != 'b')
                    name = name.Substring(1);
                var destinations = data[1].Split(", ");
                
                if (!Modules.ContainsKey(name))
                {
                    Modules[name] = 
                        type == '%' ? new FlipFlopModule(name) 
                        : type == '&' ? new ConjuctionModule(name) 
                        : new BroadcastModule();
                    destinationsInfo[name] = destinations;
                }
            }

            SinkModule endModule = null;
            foreach (var module in Modules.Values)
            {
                var destinations = destinationsInfo[module.Name];
                foreach (var destName in destinations)
                {
                    GateModule dest;
                    if (!Modules.ContainsKey(destName))
                        dest = endModule = endModule ?? new SinkModule(destName);
                    else
                        dest = Modules[destName];

                    module.Destinations.Add(dest);
                    if (dest.GetType() == typeof(ConjuctionModule))
                        ((ConjuctionModule)dest).Inputs.Add(module.Name, false);
                }
            }

            if (endModule != null)
                Modules[endModule.Name] = endModule;
        }

        static string input = @""; //paste it manually from the page
    }
}
