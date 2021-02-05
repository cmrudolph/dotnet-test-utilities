using DbUp.Engine;
using FluentAssertions;
using TestLibrary.Database;
using Xunit;

namespace TestLibrary.SelfTest
{
    public sealed class TestSqlEmbeddedScriptFactory
    {
        [Fact]
        public void GettingEmbeddedSqlScripts_DiscoversAllScripts()
        {
            var resources = SqlEmbeddedScriptFactory.GetEmbeddedSqlScripts(
                typeof(SqlEmbeddedScriptFactory).Assembly,
                "TestLibrary.SelfTest.Resources");

            resources.Should().BeEquivalentTo(new[]
            {
                new SqlScript("Tables.Table1", "T1"),
                new SqlScript("Tables.Table2", "T2"),
                new SqlScript("Scripts.Script1", "S1"),
                new SqlScript("Scripts.Script2", "S2"),
                new SqlScript("Tacos.Taco1", "Tacos!"),
            });
        }
    }
}
