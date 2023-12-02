using XmlTree;

namespace Tests
{
	[TestClass]
	public class Comments
	{
		static readonly ParserOptions options = new ParserOptions { comments = CommentOptions.Include };

		[TestMethod]
		public void Only_comment()
		{
			var doc = Parse.String("<!-- Include me -->", options);
			Assert.AreEqual(1, doc.nodes.Count);
			Assert.AreEqual("!--", doc.nodes[0].tag);
			Assert.AreEqual("Include me", doc.nodes[0].value);
		}

		[TestMethod]
		public void Div_containing_comment()
		{
			var doc = Parse.String("<div><!-- Include me --></div>", options);
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div", div.tag);
			Assert.AreEqual("", div.value);
			Assert.IsNotNull(div.children);
			Assert.AreEqual(1, div.children.Count);
			var comment = div.children[0];
			Assert.AreEqual("!--", comment.tag);
			Assert.AreEqual("Include me", comment.value);
		}

		[TestMethod]
		public void Flat()
		{
			var doc = Parse.String(@"
				<!-- Start comment -->
				<test-1></test-1>
				<test-2 class=""example""/>
				<!-- Middle comment -->
				<test-3 class=""example"" id=""lorem ipsum""/>
				<test-4 class=example id='lorem ipsum'>
					Hello World
				</test-4>
				<!-- End comment -->
				", options);
			Assert.AreEqual(7, doc.nodes.Count);

			Assert.AreEqual("!--", doc.nodes[0].tag);
			Assert.AreEqual("test-1", doc.nodes[1].tag);
			Assert.AreEqual("test-2", doc.nodes[2].tag);
			Assert.AreEqual("!--", doc.nodes[3].tag);
			Assert.AreEqual("test-3", doc.nodes[4].tag);
			Assert.AreEqual("test-4", doc.nodes[5].tag);
			Assert.AreEqual("!--", doc.nodes[6].tag);

			Assert.AreEqual("Start comment", doc.nodes[0].value);
			Assert.AreEqual("", doc.nodes[1].value);
			Assert.AreEqual("", doc.nodes[2].value);
			Assert.AreEqual("Middle comment", doc.nodes[3].value);
			Assert.AreEqual("", doc.nodes[4].value);
			Assert.AreEqual("Hello World", doc.nodes[5].value);
			Assert.AreEqual("End comment", doc.nodes[6].value);

			// test 1
			Assert.IsNull(doc.nodes[1].attributes);

			// test 2
			Assert.IsNotNull(doc.nodes[2].attributes);
			Assert.AreEqual(1, doc.nodes[2].attributes.Count);
			Assert.AreEqual("example", doc.nodes[2].attributes["class"]);

			// test 3
			Assert.IsNotNull(doc.nodes[4].attributes);
			Assert.AreEqual(2, doc.nodes[4].attributes.Count);
			Assert.AreEqual("example", doc.nodes[4].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[4].attributes["id"]);

			// test 4
			Assert.IsNotNull(doc.nodes[5].attributes);
			Assert.AreEqual(2, doc.nodes[5].attributes.Count);
			Assert.AreEqual("example", doc.nodes[5].attributes["class"]);
			Assert.AreEqual("lorem ipsum", doc.nodes[5].attributes["id"]);
		}

		[TestMethod]
		public void Hierarchical()
		{
			var doc = Parse.String(w3SchoolsFoodMenu, options);
			Assert.AreEqual(3, doc.nodes.Count);

			// First comment
			Assert.AreEqual("!--", doc.nodes[0].tag);
			Assert.AreEqual("Include me! :)", doc.nodes[0].value);

			// Last comment
			Assert.AreEqual("!--", doc.nodes[2].tag);
			Assert.AreEqual("Goodbye world!", doc.nodes[2].value);

			var menuNode = doc.nodes[1];
			Assert.AreEqual("breakfast_menu", menuNode.tag);
			Assert.IsNotNull(menuNode.children);
			Assert.AreEqual(6, menuNode.children.Count);
			for (int i = 0; i < 6; i++)
			{
				// The 5th element is a comment
				if (i == 4)
				{
					Assert.AreEqual("!--", menuNode.children[i].tag);
					Assert.AreEqual("I will be included in the array too!", menuNode.children[i].value);
				}
				// All others are just regular menu items
				else
				{
					var foodNode = menuNode.children[i];
					Assert.AreEqual("food", foodNode.tag);
					Assert.IsNotNull(foodNode.children);
					Assert.AreEqual(4, foodNode.children.Where(x => x.tag != "!--").Count(), $"Expecter element {i} to have 4 items");
					Assert.AreEqual("name", foodNode.children[0].tag);
					Assert.AreEqual("price", foodNode.children[1].tag);
					Assert.AreEqual("description", foodNode.children[2].tag);
					Assert.AreEqual("calories", foodNode.children[3].tag);
				}
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

			// Note the skipped comment

			Assert.AreEqual("Homestyle Breakfast", menuNode.children[5].children[0].value);
			Assert.AreEqual("$6.95", menuNode.children[5].children[1].value);
			Assert.AreEqual("Two eggs, bacon or sausage, toast, and our ever-popular hash browns", menuNode.children[5].children[2].value);
			Assert.AreEqual("950", menuNode.children[5].children[3].value);
		}

		// https://www.w3schools.com/xml/simple.xml
		const string w3SchoolsFoodMenu = @"
			<!-- Include me! :) -->
			<breakfast_menu>
			  <food>
				<name>Belgian Waffles</name>
				<price>$5.95</price>
				<description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
				<calories>650</calories>
				<!-- You should totally include me as well -->
			  </food>
			  <food>
				<name>Strawberry Belgian Waffles</name>
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
			  <!-- I will be included in the array too! -->
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