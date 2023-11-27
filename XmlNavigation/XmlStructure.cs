using System;
using System.Collections.Generic;
using System.IO;

namespace XmlNavigation
{
	public class XmlStructure
	{
		public string version = "1.0";
		public string xml = "";
		public List<XmlNode> nodes = new List<XmlNode>();
		public XmlError error;
		public int errorIndex;



		internal XmlStructure(string xml) => this.xml = xml;

		internal void SetError(XmlError error, int index)
		{
			this.error = error;
			this.errorIndex = index;
		}
	}
}
