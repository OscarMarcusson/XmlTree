using System;
using System.Collections.Generic;
using System.Text;

namespace XmlNavigation
{
	internal static class NodeParser
	{
		// TODO: Replace this with some proper options, obviously
		static bool allowGoodAttributeEscapes = true;


		internal static void Parse(XmlStructure doc, List<XmlNode> parent, string xml, ref int i)
		{
			var builder = new StringBuilder(64);
			xml.SkipWhitespace(ref i);
			if (xml[i] != '<')
			{
				// TODO: Parse error, no idea what the hell they gave us now
				throw new NotImplementedException("ASD6543543: " + xml.Substring(i));
			}

			if(builder.Length > 0)
			{
				var text = builder.ToString().Trim();
				if(text.Length > 0)
					parent.Add(new XmlNode() { value = text });
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

						var value = builder.ToString();
						if (allowGoodAttributeEscapes) // TODO: Add escapes for the escapes, "\\\\n" == "\\n"
							value = value.Replace("\\n", "\n").Replace("\\t", "\t");
						attributes[key] = value;
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

			// TODO: Parse eror, something like `<div class="example"` at the end of the file
			if (xml[i] != '>')
				throw new NotImplementedException("KE246842");
			i++;
			xml.SkipWhitespace(ref i);

			// Unclosed tag at the end of a file, I guess we'll accept it for now, kind of self closing in a way
			if (i > xml.Length)
			{
				parent.Add(new XmlNode
				{
					tag = tag,
					attributes = attributes,
				});
				return;
			}



			// Now here we finally go, the actual content parsing
			var node = new XmlNode
			{
				tag = tag,
				attributes = attributes,
			};
			parent.Add(node);

			
			while(i < xml.Length && xml[i] != '<')
				builder.Append(xml[i++]);

			i++;
			xml.SkipWhitespace(ref i);
			if(i > xml.Length)
			{
				// TODO: Parse error, somelike like `<div>Hello` or `<div>Hello<` at the end of the file
				throw new NotImplementedException("SDlk684646456");
			}

			// A closing tag now means this is a simple value element
			if (xml[i] == '/')
			{
				node.value = builder.ToString().Trim();
				builder.Clear();
				i++;
				xml.SkipWhitespace(ref i);
				while (i < xml.Length && xml[i].IsIdentifier())
					builder.Append(xml[i++]);

				if (builder.ToString() != node.tag) 
				{
					doc.SetError(XmlError.ClosingMissmatch, xml.LastIndexOf('<', i));
					return;
				}
				xml.SkipWhitespace(ref i);
				if(i > xml.Length || xml[i] != '>')
				{
					// Missing last >, like `<div>Example</div`
					throw new NotImplementedException("SDKO61584");
				}
				i++;
				xml.SkipWhitespace(ref i);
			}
			// Else, we have some actual hierarchical content less goooooo
			else
			{
				node.children = new List<XmlNode>();
				var depth = 1;
			}
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
			i++;
			xml.SkipWhitespace(ref i);
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
