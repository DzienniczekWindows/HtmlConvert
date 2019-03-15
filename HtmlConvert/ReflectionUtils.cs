using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace HtmlConvert
{
    public static class ReflectionUtils
    {
        public static void SetConvertedValue(this PropertyInfo info, object instance, object value)
        {
            object convertedValue = null;
            if (value.GetType() == info.PropertyType)
            {
                convertedValue = value;
            }else if (typeof(IConvertible).IsAssignableFrom(value.GetType()))
            {
                convertedValue = Convert.ChangeType(value, info.PropertyType);
            }else
            {
                var dataParam = Expression.Parameter(typeof(object), "value");
                var body = Expression.Block(Expression.Convert(Expression.Convert(dataParam, info.PropertyType),
                    info.PropertyType));

                var run = Expression.Lambda(body, dataParam).Compile();
                convertedValue = run.DynamicInvoke(value);
            }

            info.SetValue(instance, convertedValue, null);
        }

        /// <summary>
        /// This Methode extends the System.Type-type to get all extended methods. It searches hereby in all assemblies which are known by the current AppDomain.
        /// </summary>
        /// <remarks>
        /// Insired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
        /// </remarks>
        /// <returns>returns MethodInfo[] with the extended Method</returns>

        public static MethodInfo[] GetExtensionMethods(this Type t)
        {
            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var query = from type in AssTypes
                where type.IsSealed && !type.IsGenericType && !type.IsNested
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.GetParameters()[0].ParameterType == t
                select method;
            return query.ToArray<MethodInfo>();
        }

        /// <summary>
        /// Extends the System.Type-type to search for a given extended MethodeName.
        /// </summary>
        /// <param name="MethodeName">Name of the Methode</param>
        /// <returns>the found Methode or null</returns>
        public static MethodInfo GetExtensionMethod(this Type t, string MethodeName)
        {
            var mi = from methode in t.GetExtensionMethods()
                where methode.Name == MethodeName
                select methode;
            if (mi.Count<MethodInfo>() <= 0)
                return null;
            else
                return mi.First<MethodInfo>();
        }
    }
}
