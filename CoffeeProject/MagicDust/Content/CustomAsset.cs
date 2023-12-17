using MagicDustLibrary.Display;
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

    public static class AssetExtensions
    {
        private static string ParsePath(FromStorageAttribute attribute, string assetName)
        {
            return Path.Combine(
                        attribute.Path
                        .Select(it => it.Contains('*') ? Regex.Replace(it, "\\*", assetName) : it)
                        .ToArray()
                        );
        }

        private static object GetParameter(ParameterInfo parameter, string path, IContentStorage content)
        {
            if (parameter.ParameterType.IsArray)
            {
                if (Directory.Exists(path))
                {
                    var array = Directory.GetFiles(path).Select(it => content.GetAsset<object>(it));
                    return array;
                }
            }

            var value = content.GetAsset<object>(path);
            return value;
        }

        public static T Create<T>(string name, IContentStorage content)
        {
            foreach (var constructor in typeof(T).GetConstructors())
            {
                var parameters = constructor.GetParameters();
                var parameterList = new List<object>();

                foreach (var parameter in parameters)
                {
                    var attribute = parameter.GetCustomAttribute<FromStorageAttribute>();
                    if (attribute is null)
                    {
                        continue;
                    }
                    var path = ParsePath(attribute, name);

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
