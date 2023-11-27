using System;
using System.Collections.Generic;
using System.Text;

namespace XmlNavigation
{
	internal static class NodeParser
	{
		// TODO: Replace this with some proper options, obviously
		static bool allowGoodAttributeEscapes = true;


		internal static void Parse(List<XmlNode> parent, string xml, ref int i)
		{
			var builder = new StringBuilder(64);
			while (i < xml.Length && xml[i] != '<')
			{
				builder.Append(xml[i]);
				i++;
			}

			if(builder.Length > 0)
			{
				var text = builder.ToString().Trim();
				if(text.Length > 0)
				{
					var node = new XmlNode() { value = text };
					parent.Add(node);
				}
				builder.Clear();
			}

			if (i >= xml.Length)
				return;

			// Get tag
			for(i++; i < xml.Length; i++)
			{
				if (char.IsWhiteSpace(xml[i]) || xml[i] == '>' || xml[i] == '/')
					break;
				builder.Append(xml[i]);
			}
			var tag = builder.ToString();
			builder.Clear();
			xml.SkipWhitespace(ref i);
			if(i > xml.Length)
			{
				// TODO: Parse eror, something like `<div` at the end of the file
				throw new NotImplementedException("F7341813");
			}

			// Get attributes
			Dictionary<string, string> attributes = null;
			if (!xml[i].IsEndTag())
			{
				attributes = new Dictionary<string, string>();
				while(i < xml.Length && !xml[i].IsEndTag())
				{
					while(i < xml.Length && xml[i].IsIdentifier())
					{
						builder.Append(xml[i]);
						i++;
					}
					var key = builder.ToString();
					builder.Clear();
					xml.SkipWhitespace(ref i);
					if(i >= xml.Length)
					{
						// TODO: Parse error, `<div class` at the end of the file
						throw new NotImplementedException("KS85124855");
					}

					if (xml[i] == '=')
					{
						i++;
						xml.SkipWhitespace(ref i);
						if (xml[i].IsEndTag())
						{
							// TODO: Parse error, `<div class=/`
							throw new NotImplementedException("SO8864242");
						}
						// `class="example"`
						if (xml[i] == '"' || xml[i] == '\'')
						{
							var end = xml[i];
							for(i++; i < xml.Length; i++)
							{
								if (allowGoodAttributeEscapes && xml[i] == '\\') i++;
								else if (xml[i] == end) break;
								else builder.Append(xml[i]);
							}
							i++;
							
						}
						// `class=example`
						else
						{
							while(i < xml.Length && xml[i].IsIdentifier())
							{
								builder.Append(xml[i]);
								i++;
							}
						}

						attributes[key] = builder.ToString();
						builder.Clear();
						xml.SkipWhitespace(ref i);
					}
					else
					{
						attributes[key] = null;
						if (xml[i].IsEndTag())
							break;
					}
				}
			}

			xml.SkipWhitespace(ref i);
			if (i > xml.Length)
			{
				// TODO: Parse eror, something like `<div class="example"` at the end of the file
				throw new NotImplementedException("KE246842");
			}

			// Self closing
			if (xml[i] == '/')
			{
				ValidateSelfClosing(xml, ref i);
				parent.Add(new XmlNode
				{
					tag = tag,
					attributes = attributes,
				});
				return;
			}

			// Normal, expect potential value & end tag
			if (xml[i] != '>')
				throw new Exception("INTERNAL [I86244]: Reached content parse with invalid end of tag.\n" +
					"This should not happen, sorry. Please send this to the developers together with the HTML");
			i++;
			xml.SkipWhitespace(ref i);

			// Unclosed tag at the end of a file, I guess we'll accept it for now
			if (i > xml.Length)
			{
				parent.Add(new XmlNode
				{
					tag = tag,
					attributes = attributes,
				});
				return;
			}

			var depth = 1;
		}

		static void ValidateSelfClosing(string xml, ref int i)
		{
			i++;
			xml.SkipWhitespace(ref i);
			if (i > xml.Length)
			{
				// TODO: Parse eror, something like `<div/` at the end of the file
				throw new NotImplementedException("SP23483");
			}
			if (xml[i] != '>')
			{
				// TODO: Parse eror, something like `<div/what`
				throw new NotImplementedException("NY387383");
			}
		}

		static void SkipWhitespace(this string xml, ref int i)
		{
			while (i < xml.Length && char.IsWhiteSpace(xml[i]))
				i++;
		}


		static bool IsEndTag(this char c) => c == '>' || c == '/';


		static bool IsIdentifier(this char c) => char.IsLetterOrDigit(c) || c == '-' || c == '_';
	}
}
