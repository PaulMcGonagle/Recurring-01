using System;

namespace Generators
{
    public static class GeneratorFactory
    {
        public static IGenerator Get(string generatorType)
        {
            switch (generatorType)
            {
                case "holidays":
                    return new GeneratorHolidays();

                case "classes":
                    return new GeneratorClasses();

                default:
                    throw new NotImplementedException($"Unexpected generatorType '{generatorType}'");
            }
        }
    }
}
