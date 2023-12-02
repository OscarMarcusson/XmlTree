using System;
using System.Collections.Generic;
using System.Text;

namespace XmlTree
{
	public enum CommentOptions
	{
		Ignore,
		Include,
	}

	[Flags]
	public enum EscapeFlags
	{
		None = 0,
		ElementNames = 1 << 0,
		AttributeNames = 1 << 2,
		AttributeValues = 1 << 3,
		Text = 1 << 4,
		Everything = ElementNames | AttributeNames | AttributeValues | Text,
	}

	public class ParserOptions
	{
		public CommentOptions comments = CommentOptions.Ignore;


		/// <summary>
		///   
		/// </summary>
		public string[] selfClosingTags = Array.Empty<string>();

		/// <summary>
		///   Replace the escape tokens of the given type(s) with their literal value
		///   <code>
		///     &amp;lt;   = &lt;
		///     &amp;gt;   = &gt;
		///     &amp;amp;  = &amp;
		///     &amp;apos; = &apos;
		///     &amp;quot; = &quot;
		///   </code>
		/// </summary>
		public EscapeFlags escape = EscapeFlags.Everything;

		internal ParserOptions CreateCopy() => new ParserOptions
		{
			comments = comments,
			selfClosingTags = selfClosingTags,
			escape = escape,
		};
	}
}
