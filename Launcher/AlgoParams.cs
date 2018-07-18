namespace Launcher
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	public class AlgoParams : Dictionary<string, string>
	{
		public static AlgoParams Parse(string input)
		{
			var segments = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			var result = new AlgoParams();

			foreach (var segment in segments)
			{
				string key = Regex.Match(segment, @"^.+(?=\=)").Value;
				string value = Regex.Match(segment, @"(?<=\=).+$").Value;

				result.Add(key, value);
			}

			return result;
		}
	}
}