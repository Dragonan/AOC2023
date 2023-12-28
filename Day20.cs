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
        
        public void SendPulse()
        {
            foreach (var module in Destinations)
            {
                module.ReceivePulse(Pulse, Name);
                ModuleBuffer.PulsesCount[Pulse]++;
            }
        }

        public abstract void ReceivePulse(bool pulse, string from);
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
    }

    public class BroadcastModule : GateModule
    {
        public BroadcastModule() : base("broadcaster") { }

        public override void ReceivePulse(bool pulse, string from = "")
        {
            this.Pulse = pulse;
            ModuleBuffer.Modules.Add(this);
        }
    }

    public class SinkModule : GateModule
    {
        public SinkModule(string name) : base(name) { }

        public override void ReceivePulse(bool pulse, string from = "")
        {
            if (pulse == LowPulse)
                ModuleBuffer.HasSinkReceivedLowPulse = true;
        }
    }


    public static class ModuleBuffer
    {
        public static List<GateModule> Modules = new List<GateModule>();
        public static Dictionary<bool, int> PulsesCount = new Dictionary<bool, int> 
            { { true, 0 }, { false, 0 } };

        public static bool HasSinkReceivedLowPulse = false;

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

        static void SolvePart2()
        {
            var count = 0L;
            while (!ModuleBuffer.HasSinkReceivedLowPulse)
            {
                PressButton();
                count++;
            }
            Console.WriteLine(count);
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

        static void PressButton()
        {
            var broadcaster = (BroadcastModule)Modules["broadcaster"];
            ModuleBuffer.PulsesCount[LowPulse]++;
            broadcaster.ReceivePulse(LowPulse);
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

        static string test = 
@"broadcaster -> a
%a -> inv, con
&inv -> b
%b -> con
&con -> output";

        static string input = 
@"&sr -> hp
%sh -> lr
%jm -> pj, tf
&xr -> sn, sb, hd
%xt -> cc, tf
%br -> fm
%hd -> tp, xr
%rg -> xr, dl
%sb -> jh
%xg -> rd
%nf -> gx, gd
%pj -> tf, dk
%gq -> jm
%vv -> br
%gd -> gx
&hp -> rx
%cz -> gk, vv
&gk -> vq, vv, br, zt, dj, xg
%gr -> zn, xr
&tf -> cc, rf, kk, xt, gq
%dk -> tb, tf
%nt -> ph, gk
%fh -> xr, xs
%jh -> xr, bz
%pd -> gk, kb
%kb -> nt, gk
%fm -> dj, gk
%kr -> tf
%tp -> xr, rq
%lr -> mz, gx
&sn -> hp
%mz -> rv
%kj -> gx, hs
%rv -> gx, ck
%cr -> kk, tf
%rq -> gr, xr
%kk -> fc
%ck -> gx, nf
broadcaster -> hd, xt, kj, zt
%tt -> gf
%tb -> kr, tf
%gf -> gx, sh
%cc -> cr
%fc -> qx, tf
%dl -> xr
&gx -> mz, sh, tt, sr, kj, tk
%dj -> pd
%zt -> gk, xg
&rf -> hp
&vq -> hp
%xs -> sb, xr
%qx -> tf, gq
%bz -> xr, rg
%ph -> gk
%hs -> gx, tk
%tk -> tt
%rd -> gk, cz
%zn -> fh, xr"; //paste it manually from the page
    }
}