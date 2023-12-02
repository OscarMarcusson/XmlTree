using System;
using System.Collections.Generic;
using System.IO;

namespace XmlTree
{
	public class Document
	{
		public string docType;
		public string version = "1.0";
		public string xml = "";
		public List<Node> nodes = new List<Node>();
		public ParseError error;
		public int errorIndex;
		public string errorValue;



		internal Document(string xml) => this.xml = xml;

		internal void SetError(ParseError error, int index, string description = null)
		{
			this.error = error;
			this.errorIndex = Math.Max(0, Math.Min(xml.Length - 1, index));
			this.errorValue = description;
		}
	}
}
