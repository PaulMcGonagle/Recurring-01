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

                case "byOffset":
                    return new GeneratorOffset();

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

                case "RangeTime":
                    return new GeneratorXTimeRange();

                case "Tag":
                    return new GeneratorXTag();

                case "byOffset":
                    return new GeneratorXByOffset();

                default:
                    throw new NotImplementedException($"Unexpected generatorType '{generatorType}'");
            }
        }
    }
}
