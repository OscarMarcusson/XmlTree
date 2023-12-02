using XmlTree;

namespace Tests
{
	[TestClass]
	public class Empty_xml
	{
		[TestMethod]
		public void Empty()
		{
			var doc = Parse.String("");
			Assert.AreEqual(0, doc.nodes.Count);
		}
	}
}