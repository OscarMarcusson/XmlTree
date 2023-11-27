using System.Text;
using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class XML_prolog
	{
		static readonly Dictionary<string, Encoding> encodings = new Dictionary<string, Encoding>
		{
			{ "ANSI", Encoding.Default},
			{ "UTF-8", Encoding.UTF8 },
			{ "UTF-16", Encoding.Unicode },
			{ "UTF-32", Encoding.UTF32 },
		};


		[TestMethod]
		[DataRow("ANSI")]
		[DataRow("UTF-8")]
		[DataRow("UTF-16")]
		[DataRow("UTF-32")]
		public void Encodings(string encodingName)
		{
			var encoding = encodings[encodingName];
			var source = $"<?xml version=\"1.0\" encoding=\"{encodingName}\"?>\n<example>едц!</example>";
			var bytes = encoding.GetBytes(source);
			var doc = XmlParser.FromBytes(bytes);

			Assert.AreEqual(XmlError.None, doc.error);
			Assert.AreEqual(1, doc.nodes.Count);

			var node = doc.nodes[0];
			Assert.AreEqual("example", node.tag);
			Assert.AreEqual("едц!", node.value);
		}


		[TestMethod]
		[DataRow(@"<?", XmlError.UnexpectedEndOfFile)]
		[DataRow(@"<?xml", XmlError.UnexpectedEndOfFile)]
		[DataRow(@"<?incorrect version=""1.0"" encoding=""UTF-8""?> <example/>", XmlError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""  <example/>", XmlError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""? <example/>", XmlError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""> <example/>", XmlError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""", XmlError.UnexpectedEndOfFile)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-9999""?> <example/>", XmlError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""INCORRECT""?> <example/>", XmlError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""""?> <example/>", XmlError.NotAllowed)]
		public void Invalid(string xml, XmlError error)
		{
			var bytes = Encoding.UTF8.GetBytes(xml);
			var doc = XmlParser.FromBytes(bytes);

			Assert.AreEqual(error, doc.error);
		}
	}
}