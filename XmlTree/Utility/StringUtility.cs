using System;
using System.Collections.Generic;
using System.Text;

namespace XmlTree.Utility
{
	internal static class StringUtility
	{
		public static void SkipWhitespace(this string xml, ref int i)
		{
			while (i < xml.Length && char.IsWhiteSpace(xml[i]))
				i++;
		}

		public static void GotoNextNonWhitespace(this string xml, ref int i)
		{
			i++;
			while (i < xml.Length && char.IsWhiteSpace(xml[i]))
				i++;
		}

		public static bool IsEndTag(this char c) => c == '>' || c == '/';


		public static bool IsIdentifier(this char c) => char.IsLetterOrDigit(c) || c == '-' || c == '_';
	}
}
