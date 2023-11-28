using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class DocType_reading
	{
		[TestMethod]
		public void Html_empty()
		{
			var doc = XmlParser.FromString(@"
				<!DOCTYPE html>
				<html></html
				");

			Assert.AreEqual("html", doc.docType);
			Assert.AreEqual(1, doc.nodes.Count);
			Assert.AreEqual("html", doc.nodes[0].tag);
			Assert.AreEqual("", doc.nodes[0].value);
			Assert.IsNull(doc.nodes[0].children);
		}
	}
}