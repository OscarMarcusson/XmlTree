using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XmlNavigation
{
	public static class XmlParser
	{
		public static XmlStructure FromFile(string path)
		{
			var xml = File.ReadAllBytes(path);
			return FromBytes(xml);
		}

		public static XmlStructure FromBytes(byte[] bytes)
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

					// TODO:: Use standalone?
					var doc = FromString(xml);
					doc.version = version;
					return doc;
				}
			}

			// The standard fallback for XML is UTF-8
			var defaultEncodedXml = Encoding.UTF8.GetString(bytes);
			return FromString(defaultEncodedXml);
		}


		public static XmlStructure FromString(string xml)
		{
			var doc = new XmlStructure(xml);
			for (int i = 0; i < xml.Length;)
			{
				NodeParser.Parse(doc, doc.nodes, xml, ref i);
				if (doc.error != XmlError.None)
					break;
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
