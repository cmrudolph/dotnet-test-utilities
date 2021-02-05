using System.Collections.Generic;
using DbUp.Engine;
using FluentAssertions;
using TestLibrary.Database;
using Xunit;

namespace TestLibrary.SelfTest
{
    public sealed class TestSqlScriptSelector
    {
        [Fact]
        public void FilteringAndOrderingScripts_EmptyInclusions_NoScriptsFound()
        {
            var resources = new List<SqlScript>
            {
                new SqlScript("Table1", "T1"),
                new SqlScript("Script1", "S1"),
            };

            var results = SqlScriptSelector.FilterAndOrderScripts(resources, new string[0]);
            results.Should().BeEmpty();
        }

        [Fact]
        public void FilteringAndOrderingScripts_ExtraInclusions_ExtrasAreIgnored()
        {
            var resources = new List<SqlScript>
            {
                new SqlScript("Table1", "T1"),
                new SqlScript("Script1", "S1"),
            };

            var results = SqlScriptSelector.FilterAndOrderScripts(resources, new[] { "FOO" });
            results.Should().BeEmpty();
        }

        [Fact]
        public void FilteringAndOrderingScripts_ValidInclusions_OrderIsRespected()
        {
            var resources = new List<SqlScript>
            {
                new SqlScript("Script1", "S1"),
                new SqlScript("Table1", "T1"),
                new SqlScript("Table2", "T2"),
            };

            var inclusions = new[] { "Table2", "Table1", "Script1" };
            var results = SqlScriptSelector.FilterAndOrderScripts(resources, inclusions);

            results.Should().BeEquivalentTo(new[]
            {
                new SqlScript("00000-Table2", "T2"),
                new SqlScript("00001-Table1", "T1"),
                new SqlScript("00002-Script1", "S1"),
            });
        }

        [Fact]
        public void FilteringAndOrderingScripts_SharedNames_HandledProperlyAcrossTypes()
        {
            var resources = new List<SqlScript>
            {
                new SqlScript("Scripts.Name", "S1"),
                new SqlScript("Tables.Name", "T1"),
            };

            var inclusions = new[] { "Scripts.Name", "Tables.Name" };
            var results = SqlScriptSelector.FilterAndOrderScripts(resources, inclusions);

            results.Should().BeEquivalentTo(new[]
            {
                new SqlScript("00000-Scripts.Name", "S1"),
                new SqlScript("00001-Tables.Name", "T1"),
            });
        }
    }
}
