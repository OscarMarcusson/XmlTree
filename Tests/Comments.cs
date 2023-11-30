using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class Comments
	{
		[TestMethod]
		public void Only_comment()
		{
			var doc = XmlParser.FromString("<!-- Ignore me -->");
			Assert.AreEqual(0, doc.nodes.Count);
		}

		[TestMethod]
		public void Div_with_ignored_comment_child_becomes_self_closing()
		{
			var doc = XmlParser.FromString("<div><!-- Ignore me --></div>");
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div", div.tag);
			Assert.IsNull(div.children);
			Assert.AreEqual("", div.value);
		}

		[TestMethod]
		public void Flat()
		{
			var doc = XmlParser.FromString(@"
				<!-- Start comment -->
				<test-1></test-1>
				<test-2 class=""example""/>
				<!-- Middle comment -->
				<test-3 class=""example"" id=""lorem ipsum""/>
				<test-4 class=example id='lorem ipsum'>
					Hello World
				</test-4>
				<!-- End comment -->
				");
			Assert.AreEqual(4, doc.nodes.Count);

			Assert.AreEqual("test-1", doc.nodes[0].tag);
			Assert.AreEqual("test-2", doc.nodes[1].tag);
			Assert.AreEqual("test-3", doc.nodes[2].tag);
			Assert.AreEqual("test-4", doc.nodes[3].tag);

			Assert.AreEqual("", doc.nodes[0].value);
			Assert.AreEqual("", doc.nodes[1].value);
			Assert.AreEqual("", doc.nodes[2].value);
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
		public void Hierarchical()
		{
			var doc = XmlParser.FromString(w3SchoolsFoodMenu);
			Assert.AreEqual(1, doc.nodes.Count);
			var menuNode = doc.nodes[0];
			Assert.AreEqual("breakfast_menu", menuNode.tag);
			Assert.IsNotNull(menuNode.children);
			Assert.AreEqual(5, menuNode.children.Count);
			for (int i = 0; i < 5; i++)
			{
				var foodNode = menuNode.children[i];
				Assert.AreEqual("food", foodNode.tag);
				Assert.IsNotNull(foodNode.children);
				Assert.AreEqual(4, foodNode.children.Count, $"Expecter element {i} to have 4 items");
				Assert.AreEqual("name", foodNode.children[0].tag);
				Assert.AreEqual("price", foodNode.children[1].tag);
				Assert.AreEqual("description", foodNode.children[2].tag);
				Assert.AreEqual("calories", foodNode.children[3].tag);
			}

			Assert.AreEqual("Belgian Waffles", menuNode.children[0].children[0].value);
			Assert.AreEqual("$5.95", menuNode.children[0].children[1].value);
			Assert.AreEqual("Two of our famous Belgian Waffles with plenty of real maple syrup", menuNode.children[0].children[2].value);
			Assert.AreEqual("650", menuNode.children[0].children[3].value);

			Assert.AreEqual("Strawberry Belgian Waffles", menuNode.children[1].children[0].value);
			Assert.AreEqual("$7.95", menuNode.children[1].children[1].value);
			Assert.AreEqual("Light Belgian waffles covered with strawberries and whipped cream", menuNode.children[1].children[2].value);
			Assert.AreEqual("900", menuNode.children[1].children[3].value);

			Assert.AreEqual("Berry-Berry Belgian Waffles", menuNode.children[2].children[0].value);
			Assert.AreEqual("$8.95", menuNode.children[2].children[1].value);
			Assert.AreEqual("Light Belgian waffles covered with an assortment of fresh berries and whipped cream", menuNode.children[2].children[2].value);
			Assert.AreEqual("900", menuNode.children[2].children[3].value);

			Assert.AreEqual("French Toast", menuNode.children[3].children[0].value);
			Assert.AreEqual("$4.50", menuNode.children[3].children[1].value);
			Assert.AreEqual("Thick slices made from our homemade sourdough bread", menuNode.children[3].children[2].value);
			Assert.AreEqual("600", menuNode.children[3].children[3].value);

			Assert.AreEqual("Homestyle Breakfast", menuNode.children[4].children[0].value);
			Assert.AreEqual("$6.95", menuNode.children[4].children[1].value);
			Assert.AreEqual("Two eggs, bacon or sausage, toast, and our ever-popular hash browns", menuNode.children[4].children[2].value);
			Assert.AreEqual("950", menuNode.children[4].children[3].value);
		}

		// https://www.w3schools.com/xml/simple.xml
		const string w3SchoolsFoodMenu = @"
			<!-- Ignore me! :) -->
			<breakfast_menu>
			  <food>
				<name>Belgian Waffles</name> <!-- Ignore me too -->
				<price>$5.95</price>
				<description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
				<calories>650</calories>
				<!-- You should totally ignore me as well -->
			  </food>
			  <food>
				<name>Strawberry Belgian Waffles<!-- I'm not a value, ignore me --></name>
				<price>$7.95</price>
				<description>Light Belgian waffles covered with strawberries and whipped cream</description>
				<calories>900</calories>
			  </food>
			  <food>
				<name>Berry-Berry Belgian Waffles</name>
				<price>$8.95</price>
				<description>Light Belgian waffles covered with an assortment of fresh berries and whipped cream</description>
				<calories>900</calories>
			  </food>
			  <food>
				<name>French Toast</name>
				<price>$4.50</price>
				<description>Thick slices made from our homemade sourdough bread</description>
				<calories>600</calories>
			  </food>
			  <!-- Not an element in the array, skip me too -->
			  <food>
				<name>Homestyle Breakfast</name>
				<price>$6.95</price>
				<description>Two eggs, bacon or sausage, toast, and our ever-popular hash browns</description>
				<calories>950</calories>
			  </food>
			</breakfast_menu>
			<!-- Goodbye world! -->
		";
	}
}