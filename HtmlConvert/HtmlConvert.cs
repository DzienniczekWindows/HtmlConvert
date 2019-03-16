using HtmlAgilityPack;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HtmlConvert
{
    public static class HtmlConvert
    {
        private static string GetResult(HtmlNode node, HtmlPropertyAttribute htmlProperty)
        {
            string result;
            if (htmlProperty.Attribute != null)
            {
                result = node.GetAttributeValue(htmlProperty.Attribute, null);
            }
            else
            {
                result = node.InnerText;
            }

            if (htmlProperty.Regex != null)
                result = Regex.Match(result, htmlProperty.Regex).Value;
            return result;
        }

        public static T DeserializeObject<T>(string source)
        {
            return (T)DeserializeObject(typeof(T), source);
        }

        public static object DeserializeObject(Type type, string source)
        {
            if(!type.IsHtmlConvertible())
                throw new Exception("This type cannot be converted from html!");

            var html = new HtmlDocument();
            html.LoadHtml(source);
            var obj = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties())
            {
                var htmlProperty = property.GetCustomAttribute<HtmlPropertyAttribute>();
                if (htmlProperty != null)
                {
                    // TODO do casts better
                    if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        IEnumerable<HtmlNode> nodes = html.DocumentNode.QuerySelectorAll(htmlProperty.CssQuery);
                        if (nodes?.Any() != true)
                            continue;

                        var listItemType = property.PropertyType.GetGenericArguments()[0];

                        var newList = nodes.Select(x => Convert.ChangeType(GetResult(x, htmlProperty), listItemType));

                        // Invoke linq's cast by reflection to avoid generic type
                        var enumerable = typeof(IEnumerable).GetExtensionMethod("Cast").MakeGenericMethod(listItemType).Invoke(newList, new[] { newList });

                        // create new list from enumerable
                        property.SetConvertedValue(obj, Activator.CreateInstance(typeof(List<>).MakeGenericType(listItemType), new[] { enumerable }));
                        continue;
                    }

                    HtmlNode node = html.DocumentNode.QuerySelector(htmlProperty.CssQuery);
                    if(node == null)
                        continue;

                    property.SetConvertedValue(obj, GetResult(node, htmlProperty));
                }
            }

            return obj;
        }
    }
}