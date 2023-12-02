using XmlTree;

namespace Tests
{
	[TestClass]
	public class Escape
	{
		[TestMethod]
		public void Element_names()
		{
			var doc = Parse.String("<div&lt;>div&lt;</div&lt;>", new ParserOptions { escape = EscapeFlags.ElementNames });
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div<", div.tag);
			Assert.AreEqual("div&lt;", div.value);
			Assert.IsNull(div.attributes);
			Assert.IsNull(div.children);
		}

		[TestMethod]
		public void Element_text()
		{
			var doc = Parse.String("<div&lt;>div&lt;</div&lt;>", new ParserOptions { escape = EscapeFlags.Text });
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div&lt;", div.tag);
			Assert.AreEqual("div<", div.value);
			Assert.IsNull(div.attributes);
			Assert.IsNull(div.children);
		}

		[TestMethod]
		public void Attribute_name()
		{
			var doc = Parse.String("<div&lt; &lt;='&lt;'>div&lt;</div&lt;>", new ParserOptions { escape = EscapeFlags.AttributeNames });
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div&lt;", div.tag);
			Assert.AreEqual("div&lt;", div.value);
			Assert.IsNull(div.children);

			Assert.IsNotNull(div.attributes);
			Assert.AreEqual(1, div.attributes.Count);
			Assert.IsTrue(div.attributes.TryGetValue("<", out var value));
			Assert.AreEqual("&lt;", value);
		}

		[TestMethod]
		public void Attribute_value()
		{
			var doc = Parse.String("<div&lt; &lt;='&lt;'>div&lt;</div&lt;>", new ParserOptions { escape = EscapeFlags.AttributeValues });
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div&lt;", div.tag);
			Assert.AreEqual("div&lt;", div.value);
			Assert.IsNull(div.children);

			Assert.IsNotNull(div.attributes);
			Assert.AreEqual(1, div.attributes.Count);
			Assert.IsTrue(div.attributes.TryGetValue("&lt;", out var value));
			Assert.AreEqual("<", value);
		}
	}
}