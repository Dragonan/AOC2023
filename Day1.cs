using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
					
public class Day1
{
	public static void Solve()
	{
		//var client = new HttpClient();
		//var calibrationDoc = await client.GetStreamAsync("https://adventofcode.com/2023/day/1/input");
		var lines = calibrationDoc.Split('\n');
		var sum = 0;
		var regexPattern = "one|two|three|four|five|six|seven|eight|nine|\\d";
		var spelledDigits = new Dictionary<string,string>() {
			{"one","1"},{"two","2"},{"three","3"},
			{"four","4"},{"five","5"},{"six","6"},
			{"seven","7"},{"eight","8"},{"nine","9"}
		};
		
		foreach (var line in lines) {
			var firstDigit = Regex.Match(line,regexPattern).Value;
			var lastDigit = Regex.Match(line,regexPattern,RegexOptions.RightToLeft).Value;
			firstDigit = spelledDigits.ContainsKey(firstDigit) ? spelledDigits[firstDigit] : firstDigit;
			lastDigit = spelledDigits.ContainsKey(lastDigit) ? spelledDigits[lastDigit] : lastDigit;
			var number = firstDigit + lastDigit;
			sum += int.Parse(number);
		}
		
		Console.WriteLine(sum);
	}
	
	static string calibrationDoc = "";
}
