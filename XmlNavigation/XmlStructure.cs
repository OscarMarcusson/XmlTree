﻿using System;
using System.Collections.Generic;
using System.IO;

namespace XmlNavigation
{
	public class XmlStructure
	{
		public string docType;
		public string version = "1.0";
		public string xml = "";
		public List<XmlNode> nodes = new List<XmlNode>();
		public XmlError error;
		public int errorIndex;
		public string errorValue;



		internal XmlStructure(string xml) => this.xml = xml;

		internal void SetError(XmlError error, int index, string description = null)
		{
			this.error = error;
			this.errorIndex = Math.Max(0, Math.Min(xml.Length - 1, index));
			this.errorValue = description;
		}
	}
}
