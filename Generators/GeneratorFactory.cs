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
                case "calendar":
                    return new GeneratorCalendar();

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
                case "CompositeSchedule":
                    return new GeneratorXCompositeSchedule();

                case "RangeDate":
                    return new GeneratorXRangeDate();

                case "RangeTime":
                    return new GeneratorXRangeTime();

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
