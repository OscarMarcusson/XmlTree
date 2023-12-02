using System;
using System.Collections.Generic;
using System.Text;

namespace XmlTree
{
	public enum ParseError
	{
		None = 0,
		ClosingMissmatch = 1,
		UnexpectedEndOfFile = 2,
		Malformed = 3,
		NotAllowed = 4,
	}
}
