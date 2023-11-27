using System;
using System.Collections.Generic;
using System.Text;

namespace XmlNavigation
{
	public enum XmlError
	{
		None = 0,
		ClosingMissmatch = 1,
		UnexpectedEndOfFile = 2,
		Malformed = 3,
		NotAllowed = 4,
	}
}
