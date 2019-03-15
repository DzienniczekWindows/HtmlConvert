﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;

namespace HtmlConvert
{
    public static class HtmlConvert
    {
        public static T DeserializeObject<T>(string source)
        {
            var type = typeof(T);

            var html = new HtmlDocument();
            html.LoadHtml(source);
            T obj = (T)Activator.CreateInstance(type);

            foreach (var property in type.GetProperties())
            {
                var htmlProperty = property.GetCustomAttribute<HtmlPropertyAttribute>();
                if (htmlProperty != null)
                {
                    HtmlNode node = html.DocumentNode.QuerySelector(htmlProperty.CssQuery);
                    string result;

                    if (htmlProperty.Attribute != null)
                    {
                        result = node.GetAttributeValue(htmlProperty.Attribute, null);
                    }
                    else
                    {
                        result = node.InnerText;
                    }
                    property.SetValue(obj, Convert.ChangeType(result, property.PropertyType));
                }
            }

            return obj;
        }
    }
}