using System;
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
        <p class='bool'>false</p>
      </body>
  </html>");

            Assert.Equal("HtmlConvert", testObject.Title);
            Assert.Equal("HTML to .NET object converter", testObject.Description);
            Assert.Equal(3, testObject.Number);
            Assert.False(testObject.Bool);
        }

        class TestObject
        {
            [HtmlProperty(".title")]
            public string Title { get; set; }

            [HtmlProperty("html body div p:last-child")]
            public string Description { get; set; }

            [HtmlProperty(".number")]
            public int Number { get; set; }

            [HtmlProperty(".bool")]
            public bool Bool { get; set; }
        }
    }
}
