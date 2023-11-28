using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class DocType_reading
	{
		[TestMethod]
		[DataRow("<!DOCTYPE html>", "html")]
		[DataRow("<!DOCTYPE test>", "test")]
		[DataRow("<!DOCTYPE hello-world>", "hello-world")]
		public void Only_doctype(string xml, string doctype)
		{
			var doc = XmlParser.FromString(xml);

			Assert.AreEqual(doctype, doc.docType);
			Assert.AreEqual(0, doc.nodes.Count);
		}


		[TestMethod]
		public void Skips_start_comment()
		{
			var doc = XmlParser.FromString(@"
				<!-- Skip me-->
				<!DOCTYPE html>
				");

			Assert.AreEqual("html", doc.docType);
			Assert.AreEqual(0, doc.nodes.Count);
		}


		[TestMethod]
		public void Skips_multiple_start_comments()
		{
			var doc = XmlParser.FromString(@"
				<!-- Skip me-->
				<!--
					And also skip me!
				-->
				<!DOCTYPE html>
				");

			Assert.AreEqual("html", doc.docType);
			Assert.AreEqual(0, doc.nodes.Count);
		}

		[TestMethod]
		public void Html_empty()
		{
			var doc = XmlParser.FromString(@"
				<!DOCTYPE html>
				<html></html>
				");

			Assert.AreEqual("html", doc.docType);
			Assert.AreEqual(1, doc.nodes.Count);
			Assert.AreEqual("html", doc.nodes[0].tag);
			Assert.AreEqual("", doc.nodes[0].value);
			Assert.IsNull(doc.nodes[0].children);
		}

		[TestMethod]
		public void Error_for_doctype_within_content()
		{
			var doc = XmlParser.FromString(@"
				<html></html>
				<!DOCTYPE html>
				");

			Assert.AreEqual(XmlError.NotAllowed, doc.error);
		}
	}
}