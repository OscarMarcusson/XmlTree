using System.Text;
using XmlTree;

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
			var doc = Parse.Bytes(bytes);

			Assert.AreEqual(ParseError.None, doc.error);
			Assert.AreEqual(1, doc.nodes.Count);

			var node = doc.nodes[0];
			Assert.AreEqual("example", node.tag);
			Assert.AreEqual("едц!", node.value);
		}


		[TestMethod]
		[DataRow(@"<?", ParseError.UnexpectedEndOfFile)]
		[DataRow(@"<?xml", ParseError.UnexpectedEndOfFile)]
		[DataRow(@"<?incorrect version=""1.0"" encoding=""UTF-8""?> <example/>", ParseError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""  <example/>", ParseError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""? <example/>", ParseError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""> <example/>", ParseError.Malformed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-8""", ParseError.UnexpectedEndOfFile)]
		[DataRow(@"<?xml version=""1.0"" encoding=""UTF-9999""?> <example/>", ParseError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""INCORRECT""?> <example/>", ParseError.NotAllowed)]
		[DataRow(@"<?xml version=""1.0"" encoding=""""?> <example/>", ParseError.NotAllowed)]
		public void Invalid(string xml, ParseError error)
		{
			var bytes = Encoding.UTF8.GetBytes(xml);
			var doc = Parse.Bytes(bytes);

			Assert.AreEqual(error, doc.error);
		}
	}
}