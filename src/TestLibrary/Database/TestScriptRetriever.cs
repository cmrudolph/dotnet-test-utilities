using System.IO;
using System.Reflection;

namespace TestLibrary.Database
{
    /// <summary>
    /// Script retriever that is able to locate ad hoc test script files embedded within the test project. This
    /// exists to support scenarios where a test method or fixture would like to invoke a SQL script (e.g. to
    /// load data) as part of its flow.
    /// </summary>
    public sealed class TestScriptRetriever
    {
        private readonly Assembly _assembly;
        private readonly string _rootNamespace;

        public TestScriptRetriever(Assembly assembly, string rootNamespace)
        {
            _assembly = assembly;
            _rootNamespace = rootNamespace;
        }

        public string ReadTestScript(string name)
        {
            string fullName = $"{_rootNamespace}.{name}.sql";
            Stream stream = _assembly.GetManifestResourceStream(fullName);

            if (stream != null)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string content = sr.ReadToEnd();
                    return content;
                }
            }

            return null;
        }
    }
}
