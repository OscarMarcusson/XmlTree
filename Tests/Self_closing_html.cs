using XmlTree;

namespace Tests
{
	[TestClass]
	public class Self_closing_html
	{
		[TestMethod]
		public void From_doctype()
		{
			var doc = Parse.String($"<!DOCTYPE html>{html}");
			TestHtml(doc);
		}

		[TestMethod]
		public void From_options()
		{
			var doc = Parse.String(html, DefaultParserOptions.HTML);
			TestHtml(doc);
		}

		[TestMethod]
		public void Error_for_standard_xml()
		{
			var doc = Parse.String(html, DefaultParserOptions.XML);
			Assert.AreEqual(ParseError.ClosingMissmatch, doc.error, "Expected closing missmatch since we never close the links in head, so the </head> does not match <link>");
		}



		private static void TestHtml(Document doc)
		{
			Assert.AreEqual(ParseError.None, doc.error);
			Assert.AreEqual(1, doc.nodes.Count);
			var html = doc.nodes[0];
			Assert.AreEqual("html", html.tag);
			Assert.IsNotNull(html.children);
			Assert.AreEqual(2, html.children.Count);


			var head = html.children[0];
			Assert.AreEqual("head", head.tag);
			Assert.IsNotNull(head.children);
			Assert.AreEqual(3, head.children.Count);
			Assert.AreEqual("title", head.children[0].tag);
			Assert.AreEqual("Title", head.children[0].value);

			for (int i = 1; i < 3; i++)
			{
				Assert.AreEqual("link", head.children[i].tag);
				Assert.AreEqual("", head.children[i].value);
				Assert.IsNotNull(head.children[i].attributes);
				Assert.AreEqual(2, head.children[i].attributes.Count);

				Assert.IsTrue(head.children[1].attributes.TryGetValue("rel", out var rel), "rel attribute");
				Assert.AreEqual("stylesheet", rel);
			}
			Assert.IsTrue(head.children[1].attributes.TryGetValue("href", out var href), "href attribute");
			Assert.AreEqual("style-1.css", href);

			Assert.IsTrue(head.children[2].attributes.TryGetValue("href", out href), "href attribute");
			Assert.AreEqual("style-2.css", href);


			var body = html.children[1];
			Assert.AreEqual("body", body.tag);
			Assert.IsNotNull(body.children);
			Assert.AreEqual(2, body.children.Count);

			var img = body.children[0];
			Assert.AreEqual("img", img.tag);
			Assert.AreEqual("", img.value);
			Assert.IsNotNull(img.attributes);
			Assert.AreEqual(3, img.attributes.Count);

			Assert.IsTrue(img.attributes.TryGetValue("src", out var src), "src attribute");
			Assert.AreEqual("test.png", src);
			Assert.IsTrue(img.attributes.TryGetValue("alt", out var alt), "alt attribute");
			Assert.AreEqual("test image", alt);
			Assert.IsTrue(img.attributes.TryGetValue("style", out var style), "style attribute");
			Assert.AreEqual("width:6px;height:9px;", style);

			var p = body.children[1];
			Assert.AreEqual("p", p.tag);
			Assert.AreEqual("Lorem ipsum", p.value);
		}

		const string html = @"
			<html>
				<head>
					<title>Title</title>
					<link rel=""stylesheet"" href=""style-1.css"">
					<link rel=""stylesheet"" href=""style-2.css"">
				</head>  
				<body>
					<img src=""test.png"" alt=""test image"" style=""width:6px;height:9px;"">
					<p>Lorem ipsum</p>
				</body>
			</html>
			";
	}
}