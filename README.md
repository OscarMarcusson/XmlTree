# Xml Tree

A utility for parsing XML structures without all the hassle. 

```csharp
// Parse files
var doc = XmlTree.Parse.File("c:/dir/my-file.xml");
foreach(var rootNode in doc.nodes) {
    // Do something
}
```

```csharp
// Parse strings
var doc = XmlTree.Parse.String("<p> Hello World! </p>");
Console.WriteLine(doc.nodes[0].tag); // > p
Console.WriteLine(doc.nodes[0].value); // > Hello World!
```

```csharp
// Include or ignore comments
var options = new ParserOptions { comments = CommentOptions.Include };
var doc = XmlTree.Parse.String("<!-- Comments work! -->", options);
Console.WriteLine(doc.nodes[0].tag); // > !--
Console.WriteLine(doc.nodes[0].value); // > Comments work!
```

### What you get

* A full node tree of any given XML structure

* Options for getting / ignoring comments

* Full attribute support
  
  * With quotation marks, both `value="example"` and `value='example'`
  
  * Without quotation marks, like `value=example`

* Full self-closing support
  
  * Any `<x />` style element will be parsed correctly no matter the type of xml
  
  * Any self closing tags in HTML docs, like `<img />`, will be parsed correctly even if supplied as `<img>`
  
  * Optional custom closing tags in the parse options

* Simple value & text handling
  
  * Single value nodes, like a `<p>` element without any spans in HTML, will be parsed as a single node containing the text as its node value
  
  * Multi-value nodes, like a `<p>` element with one or more `<span>` children, will have all content as child nodes. Any free text outside of spans will be places as children without an element tag 

* Automatic encoding from the prolog for `File` and `Byte` parsing

#### Note

This is **not** a deserialization utility, although it could be turned into one should you wish. This is a tool for getting the tree structure of XML data. The tool was for example built for creating an HTML minifier, but it may be used in any XML related workflow.
