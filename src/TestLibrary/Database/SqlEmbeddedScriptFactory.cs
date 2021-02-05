using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DbUp.Engine;

namespace TestLibrary.Database
{
    /// <summary>
    /// Factory responsible for discovering SQL scripts embedded in a particular corner of the given assembly. Does
    /// not perform any filtering or ordering. All SQL resources found in the specified location will be processed
    /// and returned.
    /// </summary>
    public static class SqlEmbeddedScriptFactory
    {
        public static IEnumerable<SqlScript> GetEmbeddedSqlScripts(Assembly assembly, string rootNamespace)
        {
            return assembly.GetManifestResourceNames()
                .Where(res => res.StartsWith(rootNamespace))
                .Select(res => TryCreate(assembly, rootNamespace, res))
                .Where(res => res != null)
                .ToList();
        }

        private static SqlScript TryCreate(Assembly assembly, string rootNamespace, string resourceName)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException("The resource name must be specified", nameof(resourceName));
            }

            if (!resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            {
                // Ignore resources not matching our target extension
                return null;
            }

            string withoutRootNamespace = resourceName.Replace(rootNamespace, "").TrimStart('.');

            Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                // Ignore resources with no content
                return null;
            }

            string content;
            using (var sr = new StreamReader(stream))
            {
                content = sr.ReadToEnd();
            }

            string name = withoutRootNamespace.Replace(".sql", "");

            return new SqlScript(name, content);
        }
    }
}
