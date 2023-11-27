using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class XML
	{
		[TestMethod]
		public void Empty()
		{
			var doc = XmlParser.FromString("");
			Assert.AreEqual(0, doc.nodes.Count);
		}

		[TestMethod]
		[DataRow(9, "<correct></incorrect>")]
		[DataRow(14, "<correct>Value</incorrect>")]
		[DataRow(30, "<correct class=\"Example\">Value</incorrect>")]
		[DataRow(44, "<  correct   class  = \"Example\" >   Value   < /   incorrect>")]
		public void Wrong_closing_tag(int errorIndex, string xml)
		{
			var doc = XmlParser.FromString(xml);
			Assert.AreEqual(XmlError.ClosingMissmatch, doc.error);
			Assert.AreEqual(errorIndex, doc.errorIndex);
		}

		[TestMethod]
		public void Flat()
		{
			var doc = XmlParser.FromString(@"
				<test-1></test-1>
				<test-2 class=""example""></test-2>
				<test-3 class=""example"" id=""lorem ipsum"">   Hello World    </test-3>
				<test-4 class=example id='lorem ipsum'>
					Hello World
				</test-4>
				");
			Assert.AreEqual(4, doc.nodes.Count);

			Assert.AreEqual("test-1", doc.nodes[0].tag);
			Assert.AreEqual("test-2", doc.nodes[1].tag);
			Assert.AreEqual("test-3", doc.nodes[2].tag);
			Assert.AreEqual("test-4", doc.nodes[3].tag);

			Assert.AreEqual("", doc.nodes[0].value);
			Assert.AreEqual("", doc.nodes[1].value);
			Assert.AreEqual("Hello World", doc.nodes[2].value);
			Assert.AreEqual("Hello World", doc.nodes[3].value);

			// test 1
			Assert.IsNull(doc.nodes[0].attributes);

			// test 2
			Assert.IsNotNull(doc.nodes[1].attributes);
			Assert.AreEqual(1, doc.nodes[1].attributes.Count);
			Assert.AreEqual("example", doc.nodes[1].attributes["class"]);

			// test 3
			Assert.IsNotNull(doc.nodes[2].attributes);
			Assert.AreEqual(2, doc.nodes[2].attributes.Count);
			Assert.AreEqual("example", doc.nodes[2].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[2].attributes["id"]);

			// test 4
			Assert.IsNotNull(doc.nodes[3].attributes);
			Assert.AreEqual(2, doc.nodes[3].attributes.Count);
			Assert.AreEqual("example", doc.nodes[3].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[3].attributes["id"]);
		}

		[TestMethod]
		public void Flat_self_closing()
		{
			var doc = XmlParser.FromString(@"
				<test-1/>
				<test-2 class=""example""/>
				<test-3 class=""example"" id=""lorem ipsum""/>
				<test-4 class=example id='lorem ipsum'/>
				");
			Assert.AreEqual(4, doc.nodes.Count);

			Assert.AreEqual("test-1", doc.nodes[0].tag);
			Assert.AreEqual("test-2", doc.nodes[1].tag);
			Assert.AreEqual("test-3", doc.nodes[2].tag);
			Assert.AreEqual("test-4", doc.nodes[3].tag);

			Assert.AreEqual("", doc.nodes[0].value);
			Assert.AreEqual("", doc.nodes[1].value);
			Assert.AreEqual("", doc.nodes[2].value);
			Assert.AreEqual("", doc.nodes[3].value);

			// test 1
			Assert.IsNull(doc.nodes[0].attributes);

			// test 2
			Assert.IsNotNull(doc.nodes[1].attributes);
			Assert.AreEqual(1, doc.nodes[1].attributes.Count);
			Assert.AreEqual("example", doc.nodes[1].attributes["class"]);

			// test 3
			Assert.IsNotNull(doc.nodes[2].attributes);
			Assert.AreEqual(2, doc.nodes[2].attributes.Count);
			Assert.AreEqual("example", doc.nodes[2].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[2].attributes["id"]);

			// test 4
			Assert.IsNotNull(doc.nodes[3].attributes);
			Assert.AreEqual(2, doc.nodes[3].attributes.Count);
			Assert.AreEqual("example", doc.nodes[3].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[3].attributes["id"]);
		}
	}
}