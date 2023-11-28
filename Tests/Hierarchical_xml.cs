using XmlNavigation;

namespace Tests
{
	[TestClass]
	public class Hierarchical_xml
	{
		[TestMethod]
		[DataRow(14, "<div><correct></incorrect></div>")]
		[DataRow(19, "<div><correct>Value</incorrect></div>")]
		[DataRow(35, "<div><correct class=\"Example\">Value</incorrect></div>")]
		[DataRow(49, "<div><  correct   class  = \"Example\" >   Value   < /   incorrect></div>")]
		public void Wrong_closing_tag(int errorIndex, string xml)
		{
			var doc = XmlParser.FromString(xml);
			Assert.AreEqual(XmlError.ClosingMissmatch, doc.error);
			Assert.AreEqual(errorIndex, doc.errorIndex);
		}

		[TestMethod]
		public void With_values()
		{
			var doc = XmlParser.FromString(@"
				<div-1>
					<test-1></test-1>
					<test-2 class=""example""></test-2>
					<test-3 class=""example"" id=""lorem ipsum""> Hello World </test-3>
				</div-1>
				<div-2>
					<test-4 class=example id='lorem ipsum'>Hello World</test-4>
				</div-2>
				");
			Assert.AreEqual(2, doc.nodes.Count);

			var div1 = doc.nodes[0];
			Assert.AreEqual("div-1", div1.tag);
			Assert.IsNotNull(div1.children);
			Assert.AreEqual(3, div1.children.Count);
			Assert.AreEqual("test-1", div1.children[0].tag);
			Assert.AreEqual("", div1.children[0].value);

			Assert.AreEqual("test-2", div1.children[1].tag);
			Assert.AreEqual("", div1.children[1].value);

			Assert.AreEqual("test-3", div1.children[2].tag);
			Assert.AreEqual("Hello World", div1.children[2].value);

			var test3 = div1.children[2];
			Assert.IsNotNull(test3.attributes);
			Assert.AreEqual(2, test3.attributes.Count);
			Assert.AreEqual("example", test3.attributes["class"]);
			Assert.AreEqual("lorem ipsum", test3.attributes["id"]);


			var div2 = doc.nodes[1];
			Assert.AreEqual("div-2", div2.tag);
			Assert.IsNotNull(div2.children);
			Assert.AreEqual(1, div2.children.Count);

			var test4 = div2.children[0];
			Assert.AreEqual("test-4", test4.tag);
			Assert.AreEqual("Hello World", test4.value);
			Assert.IsNotNull(test4.attributes);
			Assert.AreEqual(2, test4.attributes.Count);
			Assert.AreEqual("example", test4.attributes["class"]);
			Assert.AreEqual("lorem ipsum", test4.attributes["id"]);
		}

		[TestMethod]
		public void Self_closing()
		{
			var doc = XmlParser.FromString(@"
				<div>
					<test-1/>
					<test-2 class=""example""/>
					<test-3 class=""example"" id=""lorem ipsum""/>
					<test-4 class=example id='lorem ipsum'/>
				</div>
				");
			Assert.AreEqual(1, doc.nodes.Count);
			var div = doc.nodes[0];
			Assert.AreEqual("div", div.tag);

			Assert.IsNotNull(div.children);
			Assert.AreEqual(4, div.children.Count);

			Assert.AreEqual("test-1", div.children[0].tag);
			Assert.AreEqual("test-2", div.children[1].tag);
			Assert.AreEqual("test-3", div.children[2].tag);
			Assert.AreEqual("test-4", div.children[3].tag);

			Assert.AreEqual("", div.children[0].value);
			Assert.AreEqual("", div.children[1].value);
			Assert.AreEqual("", div.children[2].value);
			Assert.AreEqual("", div.children[3].value);

			// test 1
			Assert.IsNull(div.children[0].attributes);

			// test 2
			Assert.IsNotNull(div.children[1].attributes);
			Assert.AreEqual(1, div.children[1].attributes.Count);
			Assert.AreEqual("example", div.children[1].attributes["class"]);

			// test 3
			Assert.IsNotNull(div.children[2].attributes);
			Assert.AreEqual(2, div.children[2].attributes.Count);
			Assert.AreEqual("example", div.children[2].attributes["class"]);
			Assert.AreEqual("lorem ipsum", div.children[2].attributes["id"]);

			// test 4
			Assert.IsNotNull(div.children[3].attributes);
			Assert.AreEqual(2, div.children[3].attributes.Count);
			Assert.AreEqual("example", div.children[3].attributes["class"]);
			Assert.AreEqual("lorem ipsum", div.children[3].attributes["id"]);
		}

		[TestMethod]
		public void Food_menu_array()
		{
			var doc = XmlParser.FromString(w3SchoolsFoodMenu);
			Assert.AreEqual(1, doc.nodes.Count);
			var menuNode = doc.nodes[0];
			Assert.AreEqual("breakfast_menu", menuNode.tag);
			Assert.IsNotNull(menuNode.children);
			Assert.AreEqual(5, menuNode.children.Count);
			for(int i = 0; i < 5; i++)
			{
				var foodNode = menuNode.children[i];
				Assert.AreEqual("food", foodNode.tag);
				Assert.IsNotNull(foodNode.children);
				Assert.AreEqual(4, foodNode.children.Count);
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
			<breakfast_menu>
			  <food>
				<name>Belgian Waffles</name>
				<price>$5.95</price>
				<description>Two of our famous Belgian Waffles with plenty of real maple syrup</description>
				<calories>650</calories>
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
			  <food>
				<name>Homestyle Breakfast</name>
				<price>$6.95</price>
				<description>Two eggs, bacon or sausage, toast, and our ever-popular hash browns</description>
				<calories>950</calories>
			  </food>
			</breakfast_menu>
		";
	}
}