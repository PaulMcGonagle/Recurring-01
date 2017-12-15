using System.Runtime.InteropServices;
using Generators.Instances;
using Xunit;
using TestStack.BDDfy;
using Shouldly;

namespace Generators.Test
{
    public class GeneratorFactoryTests
    {
        public class ExpandReferencesTest
        {
            private string _generatorType;
            private System.Type _expectedSystemType;
            private IGenerateFromFile _generator;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "generatorType",
                    "expectedSystemType"
                    )
                {
                    {
                        "calendar",
                        typeof(GenerateFromFileCalendar)
                    }
                }).BDDfy();
            }

            public void GivenAGeneratorType(string generatorType)
            {
                _generatorType = generatorType;
            }

            public void AndGivenAnExpectedSystemType(System.Type expectedSystemType)
            {
                _expectedSystemType = expectedSystemType;
            }

            public void WhenGeneratorIsRetrieved()
            {
                _generator = GenerateFromFileFactory.Get(_generatorType);
            }

            public void ThenGeneratorIsExpectedType()
            {
                _generator.GetType().ShouldBe(_expectedSystemType);
            }
        }
    }
}
