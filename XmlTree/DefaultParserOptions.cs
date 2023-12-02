using System;
using System.Collections.Generic;
using System.Text;

namespace XmlTree
{
	public class DefaultParserOptions
	{
		public static ParserOptions XML => new ParserOptions();
		public static ParserOptions HTML => new ParserOptions { selfClosingTags = selfClosingHtmlTags };




		internal static readonly string[] selfClosingHtmlTags = new string[]
		{
			"area",
			"base",
			"br",
			"col",
			"embed",
			"hr",
			"img",
			"input",
			"link",
			"meta",
			"param",
			"source",
			"track",
			"wbr",
		};
	}
}
