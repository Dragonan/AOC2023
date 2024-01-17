namespace AOC2023;

public class MachineComponent
{
    public MachineComponent(string name)
    {
        Name = name;
        Count = 1;
        Connections = new List<MachineConnection>();
    }

    public string Name { get; }
    public int Count { get; private set; }
    public List<MachineConnection> Connections { get; }

    public void Merge(MachineComponent other)
    {
        this.Connections.RemoveAll(c => c.IsConnecting(other));

        this.Count += other.Count;
        var toSkip = Connections.Select(c => c.GetOtherEnd(this));
        var toAdd = other.Connections.Where(c => !c.IsConnecting(this));
        foreach(var otherConn in toAdd)
        {
            otherConn.ReplaceEnd(other, this);
            this.Connections.Add(otherConn);
        }
    }
}

public class MachineConnection
{
    public MachineConnection(MachineComponent a, MachineComponent b)
    {
        A = a;
        B = b;
    }

    public MachineComponent A { get; private set; }
    public MachineComponent B { get; private set; }

    public MachineComponent GetOtherEnd(MachineComponent start)
    {
        return start == A ? B : A;
    }

    public bool IsConnecting(MachineComponent component)
    {
        return A == component || B == component;
    }

    public void ReplaceEnd(MachineComponent original, MachineComponent replacement)
    {
        if (A == original)
            A = replacement;
        else if (B == original)
            B = replacement;
    }
}

public static class Day25
{
    static List<MachineComponent> Components = new List<MachineComponent>();
    static List<MachineConnection> Connections = new List<MachineConnection>();

    public static void Solve()
    {
        var cut = 100;
        while (cut > 3) 
        {
            Components.Clear();
            Connections.Clear();
            ParseComponents(input);
            var rand = new Random();
            while (Components.Count > 2)
            {
                var i = rand.Next(Components.Count);
                var a = Components[i];
                i = rand.Next(a.Connections.Count);
                var conn = a.Connections[i];
                var b = conn.GetOtherEnd(a);
                a.Merge(b);
                Components.Remove(b);
            }
            cut = Components[0].Connections.Count;
        }
        Console.WriteLine("Connections to cut: " + Components[0].Connections.Count);
        Console.WriteLine("Groups: " + Components[0].Count + " + " + Components[1].Count);
        Console.WriteLine(Components[0].Count * Components[1].Count);
    }



    static void ParseComponents(string text)
    {
        var lines = text.Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Split(':');
            var leftName = parts[0];
            var rightNames = parts[1].Trim().Split(' ');
            var leftComponent = GetOrAddComponent(leftName);

            foreach (var rightName in rightNames)
            {
                var rightComponent = GetOrAddComponent(rightName);
                var newConnection = new MachineConnection(leftComponent, rightComponent);
                Connections.Add(newConnection);
                leftComponent.Connections.Add(newConnection);
                rightComponent.Connections.Add(newConnection);
            }
        }
    }

    static MachineComponent GetOrAddComponent(string name)
    {
        var comp = Components.FirstOrDefault(c => c.Name == name);
        if (comp == null)
        {
            comp = new MachineComponent(name);
            Components.Add(comp);
        }

        return comp;
    }

    static string input = @""; //paste it manually from the page
}
