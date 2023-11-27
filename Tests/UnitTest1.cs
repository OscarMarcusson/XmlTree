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
		public void Flat()
		{
			var doc = XmlParser.FromString(@"
				<test>Hello</test>
				<test> </test>
				<test>
					World
				</test>
				");
			Assert.AreEqual(3, doc.nodes.Count);
			
			Assert.AreEqual("test", doc.nodes[0].tag);
			Assert.AreEqual("test", doc.nodes[1].tag);
			Assert.AreEqual("test", doc.nodes[2].tag);

			Assert.AreEqual("Hello", doc.nodes[0].value);
			Assert.AreEqual("", doc.nodes[1].value);
			Assert.AreEqual("World", doc.nodes[2].value);

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