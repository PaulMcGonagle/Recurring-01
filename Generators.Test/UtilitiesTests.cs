using System.Collections.Generic;
using Xunit;
using TestStack.BDDfy;
using Shouldly;
using Scheduler.Persistance;

namespace Generators.Test
{
    public class UtilitiesTests
    {
        public class ExpandReferencesTest
        {
            private string _sourceFile;
            private IGenerator _generator;
            private GeneratorHolidays _generatorHolidays;

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

            public void AndWhenGeneratorIsHoliday()
            {
                _generator.ShouldBeOfType<GeneratorHolidays>();

                _generatorHolidays = (GeneratorHolidays)_generator;
            }

            public void AndWhenGenerated()
            {
                _generatorHolidays.Generate(_sourceFile);
            }

        }
    }
}
