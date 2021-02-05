using System.Text.Json;

namespace TestLibrary.Utils
{
    /// <summary>
    /// Able to produce deep copies of objects as long as they conform to our expectations. This means being able to
    /// round trip through the System.Text.Json serializer implementation. Many objects used in testing (DTOs,
    /// entities, POCOs, etc.) fit this criteria and this provides a quick and easy way to get genuine copies.
    /// Working with copies is preferable to sharing instances to avoid a few types of issues:
    ///   1. Mutation: Mutating an object in one place affects other holders of the same reference
    ///   2. Reference equality: Accidentally taking a dependency on reference equality becomes easy to do
    /// </summary>
    public static class DeepCopier
    {
        public static T Copy<T>(T obj)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
        }
    }
}
