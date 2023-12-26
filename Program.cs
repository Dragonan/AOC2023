using System;
using System.Diagnostics;

namespace AOC2023
{
	public class Program
	{
		public static void Main()
		{
			var timer = new Stopwatch();
			timer.Start();


			Day1.Solve();
			
			
			timer.Stop();
			if (timer.ElapsedMilliseconds > 999)
				Console.WriteLine($"Time: {timer.Elapsed.TotalSeconds}s");
			else
				Console.WriteLine($"Time: {timer.ElapsedMilliseconds}ms");;
		}
	}
}
