using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XmlNavigation.Utility;

namespace XmlNavigation
{
	public static class XmlParser
	{
		public static XmlStructure FromFile(string path, ParserOptions options = null)
		{
			var xml = File.ReadAllBytes(path);
			return FromBytes(xml, options);
		}

		public static XmlStructure FromBytes(byte[] bytes, ParserOptions options = null)
		{
			// Resolve the prolog, if one exists, like: <?xml version="1.0" encoding="UTF-8"?>
			var i = 0;
			bytes.SkipWhitespace(ref i);
			if (bytes[i] == '<')
			{
				i++;
				bytes.SkipWhitespace(ref i);
				if (i >= bytes.Length)
					return bytes.Error(XmlError.UnexpectedEndOfFile);

				if (bytes[i] == '?')
				{
					i++;
					bytes.SkipWhitespace(ref i);
					var builder = new StringBuilder();
					while (i < bytes.Length && bytes[i] != '>' && bytes[i] != '?' && bytes[i] != '<' && bytes[i] != '/')
					{
						builder.Append((char)bytes[i++]);
						bytes.SkipNull(ref i);
					}

					if (i >= bytes.Length)
						return bytes.Error(XmlError.UnexpectedEndOfFile);

					if (bytes[i] == '<' || bytes[i] == '/') return bytes.Error(XmlError.Malformed, ((char)bytes[i]).ToString());
					if (bytes[i] != '?') return bytes.Error(XmlError.Malformed, ((char)bytes[i]).ToString());
					i++;
					bytes.SkipWhitespace(ref i);
					if (i >= bytes.Length)
						return bytes.Error(XmlError.UnexpectedEndOfFile);

					if (bytes[i] == '<' || bytes[i] == '/') return bytes.Error(XmlError.Malformed, ((char)bytes[i]).ToString());
					if (bytes[i] == '>') i++;
					bytes.SkipWhitespace(ref i);
					if (i < bytes.Length && bytes[i] == '>') i++;
					bytes.SkipWhitespace(ref i);

					var miniXml = $"<{builder}/>";
					var mainStart = i;
					var mainLength = bytes.Length - i;

					var prolog = FromString(miniXml).nodes[0];
					if (prolog.tag != "xml") return bytes.Error(XmlError.NotAllowed, prolog.tag);
					var version = prolog.GetAttribute("version", "1.0");
					var encoding = prolog.GetAttribute("encoding", "UTF-8");
					var standalone = prolog.GetAttribute("standalone", "no");
					
					string xml;
					switch (encoding.ToUpper())
					{
						case "UTF8":
						case "UTF-8":
							xml = Encoding.UTF8.GetString(bytes, mainStart, mainLength);
							break;

						case "ANSI":
							xml = Encoding.Default.GetString(bytes, mainStart, mainLength);
							break;

						case "UTF-16":
							xml = Encoding.Unicode.GetString(bytes, mainStart, mainLength);
							break;

						case "UTF-32":
							xml = Encoding.UTF32.GetString(bytes, mainStart, mainLength);
							break;

						default: return bytes.Error(XmlError.NotAllowed, encoding);
					}

					var doc = FromString(xml, options);
					doc.version = version;
					return doc;
				}
			}

			// The standard fallback for XML is UTF-8
			var defaultEncodedXml = Encoding.UTF8.GetString(bytes);
			return FromString(defaultEncodedXml, options);
		}


		public static XmlStructure FromString(string xml, ParserOptions options = null)
		{
			var optionsToUse = options?.CreateCopy() ?? DefaultParserOptions.XML;

			// Resolve the doctype, if any
			string docType = null;
			int skipped = 0;
			for(int i = 0; i < xml.Length; i++)
			{
				xml.SkipWhitespace(ref i);
				if (i >= xml.Length || xml[i] != '<') break;
				i++;
				xml.SkipWhitespace(ref i);
				if (i >= xml.Length || xml[i] != '!') break;
				i++;
				xml.SkipWhitespace(ref i);
				if (i >= xml.Length) break;

				// Special handling for comments at start
				if (xml[i] == '-')
				{
					i = xml.IndexOf("-->", i);
					if(i < 0)
					{
						var error = new XmlStructure(xml);
						error.SetError(XmlError.UnexpectedEndOfFile, xml.Length, "Missing -->");
						return error;
					}
					i += 2;
					// Comment skipped, try again
					continue;
				}

				var builder = new StringBuilder();
				while (i < xml.Length && xml[i].IsIdentifier())
					builder.Append(xml[i++]);

				if (!builder.ToString().Equals("doctype", StringComparison.OrdinalIgnoreCase))
					break;

				// This is a doctype, resolve it
				xml.SkipWhitespace(ref i);
				var start = i;
				var end = xml.IndexOf('>', start + 1);
				if(end < start)
				{
					var error = new XmlStructure(xml);
					error.SetError(XmlError.UnexpectedEndOfFile, xml.Length, "Missing >");
					return error;
				}

				docType = xml.Substring(start, end - start).Trim();
				skipped = end + 1;

				// TODO:: Use standalone?
				if (docType.Equals("html", StringComparison.OrdinalIgnoreCase))
				{
					// TODO:: If other is already set, what do? Enum to handle override / ignore etc?
					optionsToUse.selfClosingTags = DefaultParserOptions.HTML.selfClosingTags;
				}
				break;
			}


			var doc = new XmlStructure(xml);
			doc.docType = docType;
			for (int i = skipped; i < xml.Length;)
			{
				xml.SkipWhitespace(ref i);
				if (i >= xml.Length)
					break;

				NodeParser.Parse(doc, doc.nodes, xml, ref i, optionsToUse);
				if (doc.error != XmlError.None)
					break;
			}

			if(optionsToUse.escape != EscapeFlags.None)
			{
				foreach (var node in doc.nodes)
					node.ReplaceEscaped(optionsToUse.escape);
			}

			return doc;
		}



		static void SkipWhitespace(this byte[] bytes, ref int i)
		{
			while (i < bytes.Length && (char.IsWhiteSpace((char)bytes[i]) || bytes[i] == 0))
				i++;
		}
		static void SkipNull(this byte[] bytes, ref int i)
		{
			while (i < bytes.Length && bytes[i] == 0)
				i++;
		}

		static XmlStructure Error(this byte[] bytes, XmlError error, string value = null)
		{
			var xmlBuilder = new StringBuilder();
			foreach(var b in bytes)
			{
				if (b == 0)
					continue;
				xmlBuilder.Append((char)b);
			}
			var xml = xmlBuilder.ToString();
			var errorDoc = new XmlStructure(xml);
			var index = errorDoc.xml.IndexOf('<');
			if (!string.IsNullOrWhiteSpace(value))
			{
				var valueIndex = xml.IndexOf(value, index + 1);
				if (valueIndex > -1)
					index = valueIndex;
			}
			errorDoc.SetError(error, index, value);
			return errorDoc;
		}
	}
}
