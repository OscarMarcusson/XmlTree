using System;
using System.Collections.Generic;
using System.Text;

namespace XmlTree.Utility
{
	internal static class EscapeUtility
	{
		static readonly Dictionary<string, string> standard = new Dictionary<string, string>()
		{
			{ "&lt;", "<" },
			{ "&gt;", ">" },
			{ "&amp;", "&" },
			{ "&apos;", "'" },
			{ "&quot;", "\"" },
		};

		public static string ReplaceEscaped(this string input)
		{
			if (input.Length == 0)
				return input;

			foreach (var pair in standard)
				input = input.Replace(pair.Key, pair.Value);

			return input;
		}
	}
}
