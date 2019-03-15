using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlConvert
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HtmlPropertyAttribute : Attribute
    {
        public string Expression { get; set; }

        public enum ExpressionType { css, xpath }
        public ExpressionType Type { get; set; }

        public HtmlPropertyAttribute(string expression, ExpressionType type = ExpressionType.css)
        {
            Expression = expression;
            Type = type;
        }
    }
}
