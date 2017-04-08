using System;
using Generators.Instances;
using Generators.XInstances;

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

                case "schedule":
                    return new GeneratorSchedule();

                default:
                    throw new NotImplementedException($"Unexpected generatorType '{generatorType}'");
            }
        }

        public static IGeneratorX GetX(string generatorType)
        {
            switch (generatorType)
            {
                case "DateRange":
                    return new GeneratorXDateRange();

                default:
                    throw new NotImplementedException($"Unexpected generatorType '{generatorType}'");
            }
        }
    }
}
