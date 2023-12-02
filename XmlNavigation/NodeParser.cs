using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlNavigation.Utility;

namespace XmlNavigation
{
	internal static class NodeParser
	{
		// TODO: Replace this with some proper options, obviously
		static bool allowGoodAttributeEscapes = true;


		internal static void Parse(XmlStructure doc, List<XmlNode> parent, string xml, ref int i, ParserOptions options)
		{
			var builder = new StringBuilder(64);
			xml.SkipWhitespace(ref i);
			if (i >= xml.Length)
			{
				doc.SetError(XmlError.UnexpectedEndOfFile, i);
				return;
			}
			if (xml[i] != '<') { doc.SetError(XmlError.Malformed, i, "Expected <"); return; }

			// Get tag
			xml.GotoNextNonWhitespace(ref i);
			if (i > xml.Length) { doc.SetError(XmlError.UnexpectedEndOfFile, i, "Missing element tag"); return; }
			var startOfTagIndex = i;
			if (xml[i] == '!')
			{
				// Comment parsing
				if(i + 2 < xml.Length && xml[i+1] == '-' && xml[i+2] == '-')
				{
					i += 2;
					xml.GotoNextNonWhitespace(ref i);
					var commentEndIndex = xml.IndexOf("-->", i);
					if(commentEndIndex < 0) { doc.SetError(XmlError.UnexpectedEndOfFile, xml.Length, "Expected -->"); return; }

					if(options.comments == CommentOptions.Include)
					{
						var comment = xml.Substring(i, commentEndIndex - i).Trim();
						parent.Add(new XmlNode
						{
							tag = "!--",
							value = comment,
						});
					}

					i = commentEndIndex + 3;
					return;
				}

				builder.Append(xml[i]);
				xml.GotoNextNonWhitespace(ref i);
				// TODO: Special handling for --
			}
			// Read the element name
			for (; i < xml.Length; i++)
			{
				if (char.IsWhiteSpace(xml[i]) || xml[i].IsEndTag())
					break;
				builder.Append(xml[i]);
			}
			var tag = builder.ToString();
			builder.Clear();
			if (tag.Length == 0) { doc.SetError(XmlError.UnexpectedEndOfFile, i, "Missing element tag"); return; }
			if(tag.Equals("!doctype", StringComparison.OrdinalIgnoreCase)) { doc.SetError(XmlError.NotAllowed, startOfTagIndex, "The <!DOCTYPE> may only be set at the beginning of the file"); return; }

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
					while(i < xml.Length && !char.IsWhiteSpace(xml[i]) && !xml[i].IsEndTag() && xml[i] != '=')
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
						xml.GotoNextNonWhitespace(ref i);
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
						else
							i++;
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
			if (xml[i] == '/' || (xml[i] == '>' && options.selfClosingTags.Contains(tag)))
			{
				if (xml[i] == '/')
					xml.GotoNextNonWhitespace(ref i);

				if (i > xml.Length) { doc.SetError(XmlError.UnexpectedEndOfFile, i, "Expected />"); return; }
				if (xml[i] != '>') { doc.SetError(XmlError.Malformed, i, "Expected >"); return; }
				xml.GotoNextNonWhitespace(ref i);
				
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
			xml.GotoNextNonWhitespace(ref i);

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

			startOfTagIndex = i;
			xml.GotoNextNonWhitespace(ref i);
			if (i > xml.Length)
			{
				// TODO: Parse error, somelike like `<div>Hello` or `<div>Hello<` at the end of the file
				throw new NotImplementedException("SDlk684646456");
			}

			// A closing tag now means this is a simple value element
			if (xml[i] == '/')
			{
				node.value = builder.ToString().Trim();
				builder.Clear();
				xml.GotoNextNonWhitespace(ref i);

				while (i < xml.Length && xml[i].IsIdentifier())
					builder.Append(xml[i++]);

				if (builder.ToString() != node.tag) 
				{
					doc.SetError(XmlError.ClosingMissmatch, xml.LastIndexOf('<', i), $"Expected {node.tag}");
					return;
				}
				xml.SkipWhitespace(ref i);
				if(i >= xml.Length || xml[i] != '>')
				{
					// Missing last >, like `<div>Example</div`
					throw new NotImplementedException("SDKO61584");
				}
				xml.GotoNextNonWhitespace(ref i);
			}
			// Else, we have some actual hierarchical content less goooooo
			else
			{
				node.children = new List<XmlNode>();

				// Add the text up until now as a text value, like `<div> This text <span>!</span></div>`
				var prefixText = builder.ToString().Trim();
				if (prefixText.Length > 0)
					node.children.Add(new XmlNode { value = prefixText });

				i = startOfTagIndex;
				while (i < xml.Length && doc.error == XmlError.None)
				{
					// Parse the first child node
					Parse(doc, node.children, xml, ref i, options);
					if (doc.error != XmlError.None) return;
					xml.SkipWhitespace(ref i);
					if (i >= xml.Length) return;

					// Inlined text
					if (xml[i] != '<')
					{
						throw new NotImplementedException("TEXT AS66848646");
					}
					// End of this node
					if (xml[i] == '<')
					{
						var nextIndex = i + 1;
						xml.SkipWhitespace(ref nextIndex);
						if (nextIndex >= xml.Length) continue; // Let the regular parser handle this error
						if (xml[nextIndex] == '/')
						{
							// End of tag
							// TODO:: Validate value
							i = xml.IndexOf('>', nextIndex) + 1;
							if(i < nextIndex)
							{
								doc.SetError(XmlError.Malformed, nextIndex, "No closing >");
								i = nextIndex;
								return;
							}
							break;
						}
					}
				}

				// If we only have text, which may happen if we have comments + raw text,
				// then we lift that text into the node value instead
				if (node.children.All(x => x.tag.Length == 0))
				{
					// TODO:: More content aware join?
					node.value = string.Join("\n", node.children.Select(x => x.value));
					node.children = null;
				}
				// If we have nothing, for example due to comments, we clear the child list
				else if(node.children.Count == 0)
				{
					node.children = null;
				}
			}
		}
	}
}
