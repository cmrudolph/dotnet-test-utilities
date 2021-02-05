using System.Collections.Generic;
using System.Linq;
using DbUp.Engine;

namespace TestLibrary.Database
{
    /// <summary>
    /// Responsible for transforming a set of raw SQL scripts into what we need for our tests. This process involves:
    ///   1. Determining which resources to include in the tests. We do not necessarily want to load every SQL script
    ///      into the test database. This decision is left up to the fixture.
    ///   2. Determining the necessary order for the selected SQL scripts. This matters because the scripts likely
    ///      have dependency relationships that need to be respected. Specifying the order is the responsibility
    ///      of the fixture.
    /// </summary>
    public static class SqlScriptSelector
    {
        public static IEnumerable<SqlScript> FilterAndOrderScripts(IEnumerable<SqlScript> allScripts, IEnumerable<string> orderedInclusions)
        {
            var scripts = new List<SqlScript>();

            Dictionary<string, SqlScript> typedResourcesByName = allScripts.ToDictionary(r => r.Name);

            int prefixIndex = 0;
            foreach (string orderedInclusion in orderedInclusions)
            {
                string prefix = prefixIndex.ToString().PadLeft(5, '0');
                SqlScript script = typedResourcesByName.TryGetValue(orderedInclusion, out SqlScript found) ? found : null;
                
                if (script != null)
                {
                    scripts.Add(new SqlScript($"{prefix}-{script.Name}", script.Contents));
                    prefixIndex++;
                }
            }

            return scripts;
        }
    }
}
