using HtmlAgilityPack;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Fizzler.Systems.HtmlAgilityPack;
using System.Collections.Generic;

namespace HtmlConvert
{
    public static class HtmlConvert
    {
        private static string GetResult(HtmlNode node, HtmlPropertyAttribute htmlProperty)
        {
            if (htmlProperty.Attribute != null)
            {
                return node.GetAttributeValue(htmlProperty.Attribute, null);
            }
            else
            {
                return node.InnerText;
            }
        }

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
                    // TODO do casts better
                    if (typeof(IList).IsAssignableFrom(property.PropertyType))
                    {
                        IEnumerable<HtmlNode> nodes = html.DocumentNode.QuerySelectorAll(htmlProperty.CssQuery);
                        var listItemType = property.PropertyType.GetGenericArguments()[0];

                        var newList = nodes.Select(x => Convert.ChangeType(GetResult(x, htmlProperty), listItemType));

                        // Invoke linq's cast by reflection to avoid generic type
                        var enumerable = typeof(IEnumerable).GetExtensionMethod("Cast").MakeGenericMethod(listItemType).Invoke(newList, new[] { newList });

                        // create new list from enumerable
                        property.SetConvertedValue(obj, Activator.CreateInstance(typeof(List<>).MakeGenericType(listItemType), new[] { enumerable }));
                        continue;
                    }

                    HtmlNode node = html.DocumentNode.QuerySelector(htmlProperty.CssQuery);
                    property.SetConvertedValue(obj, GetResult(node, htmlProperty));
                }
            }

            return obj;
        }
    }
}