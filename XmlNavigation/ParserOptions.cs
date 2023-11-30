using System;
using System.Collections.Generic;
using System.Text;

namespace XmlNavigation
{
	public enum CommentOptions
	{
		Ignore,
		Include,
	}

	public class ParserOptions
	{
		public CommentOptions comments = CommentOptions.Ignore;
		public string[] selfClosingTags = Array.Empty<string>();



		internal ParserOptions CreateCopy() => new ParserOptions
		{
			comments = comments,
			selfClosingTags = selfClosingTags,
		};
	}
}
