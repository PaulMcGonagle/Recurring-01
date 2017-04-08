using System.Collections.Generic;
using Generators.Instances;
using Xunit;
using TestStack.BDDfy;
using Shouldly;
using Scheduler.Persistance;

namespace Generators.Test
{
    public class GeneratorFactoryTests
    {
        public class ExpandReferencesTest
        {
            private string _sourceFile;
            private IGenerator _generator;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "sourceFile"
                    )
                {
                    "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\Generators\\Sources\\Holidays.xml",
                }).BDDfy();
            }

            public void GivenASourceFile(string sourceFile)
            {
                _sourceFile = sourceFile;
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("holidays");
            }

            public void ThenGeneratorIsHoliday()
            {
                _generator.ShouldBeOfType<GeneratorHolidays>();
            }
        }
    }
}
