using System;
using System.Collections.Generic;
using Xunit;

namespace HtmlConvert.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var testObject = HtmlConvert.DeserializeObject<TestObject>(@"
            <html>
                <head></head>
                <body>
                    <div>
                        <p class='title'>HtmlConvert</p>
                        <p>HTML to .NET object converter</p>
                    </div>
                    <p class='number'>3</p>
                    <p class='numbertext'>No u, part 2</p>
                    <p class='bool'>false</p>
                    <form method='POST' name='hiddenform' action='test.php'>
                        <input type='hidden' name='hiddeninput' value='samplevalue' />
                    </form>

                    <ul>
                        <li test='it works'>First item</li>
                        <li test='lol'>Second item</li>
                        <li test=';)'>Third item</li>
                    </ul>
                    <ol>
                        <li bool='false'>1</li>
                        <li bool='true'>2</li>
                        <li bool='false'>3</li>
                    </ol>
                </body>
            </html>");

            Assert.Equal("HtmlConvert", testObject.Title);
            Assert.Equal("HTML to .NET object converter", testObject.Description);
            Assert.Equal(3, testObject.Number);
            Assert.Equal(2, testObject.NumberButInText);
            Assert.False(testObject.Bool);
            Assert.Equal("test.php", testObject.Action);
            Assert.Equal("samplevalue", testObject.InputValue);

            Assert.Equal(new List<string>() {"First item", "Second item", "Third item"}, testObject.Items);
            Assert.Equal(new List<string>() {"it works", "lol", ";)"}, testObject.Tests);
            Assert.Equal(new List<int>() {1,2,3}, testObject.Numbers);
            Assert.Equal(new List<bool>() {false,true,false}, testObject.Bools);
        }

        class TestObject
        {
            [HtmlProperty(".title")]
            public string Title { get; set; }

            [HtmlProperty("html body div p:last-child")]
            public string Description { get; set; }

            [HtmlProperty(".number")]
            public int Number { get; set; }

            [HtmlProperty(".numbertext", regex: @"\d+")]
            public int NumberButInText { get; set; }

            [HtmlProperty(".bool")]
            public bool Bool { get; set; }

            [HtmlProperty("form[name=hiddenform]", attribute: "action")]
            public string Action { get; set; }

            [HtmlProperty("input[name=hiddeninput]", attribute: "value")]
            public string InputValue { get; set; }

            [HtmlProperty("ul li")] public List<string> Items { get; set; }
            [HtmlProperty("ul li", attribute: "test")] public List<string> Tests { get; set; }
            [HtmlProperty("ol li")] public List<int> Numbers { get; set; }
            [HtmlProperty("ol li", attribute: "bool")] public List<bool> Bools { get; set; }
        }
    }
}
