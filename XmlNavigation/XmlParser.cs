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
			for (int i = 0; i < xml.Length;)
			{
				NodeParser.Parse(doc, doc.nodes, xml, ref i);
				if (doc.error != XmlError.None)
					break;
			}
			return doc;
		}
	}
}
