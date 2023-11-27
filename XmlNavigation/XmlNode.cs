using System;
using System.Collections.Generic;
using System.Text;

namespace XmlNavigation
{
	public class XmlNode
	{
		public string tag = "";
		public Dictionary<string, string> attributes;
		public string value = "";

		public List<XmlNode> children;



		public string GetAttribute(string attribute, string defaultValue)
		{
			if (attributes == null)
				return defaultValue;

			if (attributes.TryGetValue(attribute, out var value))
				return value;

			return defaultValue;
		}
	}
}
