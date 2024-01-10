using MagicDustLibrary.Display;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagicDustLibrary.Content
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromStorageAttribute(params string[] path) : Attribute
    {
        public string[] Path { get; } = path;
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class FromInputAttribute(int index) : Attribute
    {
        public int Index { get; } = index;
    }

    public static class AssetExtensions
    {
        private static string GetAndMove(IEnumerator<string> names)
        {
            var result = names.Current;
            names.MoveNext();
            return result;
        }

        private static string ParsePath(FromStorageAttribute attribute, IEnumerator<string> names)
        {
            names.MoveNext();
            return Path.Combine(
                        attribute.Path
                        .Select(it => it.Contains('*') ? Regex.Replace(it, "\\*", GetAndMove(names)) : it)
                        .ToArray()
                        );
        }

        private static object GetParameter(ParameterInfo parameter, string path, IContentStorage content)
        {
            var value = content.GetAsset<object>(path);
            return value;
        }

        public static T Create<T>(IContentStorage content, params string[] inputNames)
        {
            foreach (var constructor in typeof(T).GetConstructors())
            {
                var parameters = constructor.GetParameters();
                var parameterList = new List<object>();

                foreach (var parameter in parameters)
                {
                    if (parameter.ParameterType == typeof(IContentStorage))
                    {
                        parameterList.Add(content);
                        continue;
                    }

                    var fromInputAttribute = parameter.GetCustomAttribute<FromInputAttribute>();

                    if (fromInputAttribute is not null)
                    {
                        if (parameter.ParameterType != typeof(string))
                        {
                            throw new Exception("FromInputAttribute must be applied to string parameter");
                        }
                        parameterList.Add(inputNames[fromInputAttribute.Index]);
                        continue;
                    }

                    var attribute = parameter.GetCustomAttribute<FromStorageAttribute>();

                    if (attribute is null)
                    {
                        continue;
                    }
                    var path = ParsePath(attribute, inputNames.AsEnumerable().GetEnumerator());

                    var value = GetParameter(parameter, path, content);

                    if (value is null)
                    {
                        throw new Exception($"Failed to load content from {path}");
                    }

                    parameterList.Add(GetParameter(parameter, path, content));
                }

                if (parameterList.Count == 0)
                {
                    continue;
                }

                return (T)constructor.Invoke(parameterList.ToArray());
            }

            throw new Exception("Asset Constructor not found");
        }
    }
}
