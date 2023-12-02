using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlNavigation.Utility;

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
			// Special handling for comments
			if (tag == "!--")
			{
				return this.value.Length > 0
					? $"<!-- {this.value} -->"
					: $"<!-- -->"
					;
			}

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

		internal void ReplaceEscaped(EscapeFlags escape)
		{
			if(escape.HasFlag(EscapeFlags.ElementNames))
				tag = tag.ReplaceEscaped();

			if (escape.HasFlag(EscapeFlags.Text))
				value = value.ReplaceEscaped();

			if(attributes?.Count > 0 && (escape.HasFlag(EscapeFlags.AttributeNames) || escape.HasFlag(EscapeFlags.AttributeValues)))
			{
				var key = escape.HasFlag(EscapeFlags.AttributeNames);
				var value = escape.HasFlag(EscapeFlags.AttributeValues);
				attributes = attributes.ToDictionary(
					pair => key ? pair.Key.ReplaceEscaped() : pair.Key,
					pair => value ? pair.Value.ReplaceEscaped() : pair.Value);
			}

			if(children?.Count > 0)
			{
				foreach (var node in children)
					node.ReplaceEscaped(escape);
			}
		}
	}
}
