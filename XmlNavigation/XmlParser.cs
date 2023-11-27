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
			var xml = File.ReadAllText(path);
			return FromString(xml);
		}


		public static XmlStructure FromString(string xml)
		{
			var doc = new XmlStructure();
			var rootNode = new XmlNode();
			for (int i = 0; i < xml.Length; i++)
			{
				if (char.IsWhiteSpace(xml[i]))
					continue;

				NodeParser.Parse(doc.nodes, xml, ref i);
			}
			return doc;
		}
	}
}
