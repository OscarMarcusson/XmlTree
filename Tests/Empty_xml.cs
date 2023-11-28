using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class Empty_xml
	{
		[TestMethod]
		public void Empty()
		{
			var doc = XmlParser.FromString("");
			Assert.AreEqual(0, doc.nodes.Count);
		}
	}
}