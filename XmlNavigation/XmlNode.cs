using System;
using System.Collections.Generic;
using System.Linq;
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

		public override string ToString()
		{
			var value = !string.IsNullOrWhiteSpace(this.value)
				? this.value
				: children?.Count > 0 ? " ... " : ""
				;

			var attributesString = attributes?.Count > 0 ? $" {string.Join(" ", attributes.Select(x => $"{x.Key}=\"{x.Value}\""))}" : "";
			if (value.Length > 0)
				return $"<{tag}{attributesString}>{value}</{tag}>";
			
			if(attributesString.Length > 0)
				return $"<{tag}{attributesString}/>";

			return $"<{tag}/>";
		}
	}
}
