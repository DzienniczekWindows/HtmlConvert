using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlConvert
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HtmlPropertyAttribute : Attribute
    {
        public string CssQuery { get; set; }
        public string Attribute { get; set; }

        public HtmlPropertyAttribute(string cssQuery, string attribute = null)
        {
            CssQuery = cssQuery;
            Attribute = attribute;
        }
    }
}
