using System;
using System.Xml.Linq;
using Generators.Instances;
using Generators.XInstances;
using Generators.XScheduleInstances;

namespace Generators
{
    public static class GenerateFromFileFactory
    {
        public static IGenerateFromFile Get(string generatorType)
        {
            switch (generatorType)
            {
                case "calendar":
                    return new GenerateFromFileCalendar();

                case "classes":
                    return new GenerateFromFileClasses();

                case "terms":
                    return new GenerateFromFileTerms();

                case "schedule":
                    return new GenerateFromFileSchedule();

                case "byOffset":
                    return new GenerateFromFileOffset();

                case "timetables":
                    return new GenerateFromFileTimetable();

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

        public static IGeneratorX GetXSchedule(XElement xSchedule)
        {
            return GetXSchedule(xSchedule.Name.LocalName);
        }

        public static IGeneratorX GetXSchedule(string scheduleType)
        {
            switch (scheduleType)
            {
                case "byRangeDate":
                    return new GeneratorXByRangeDate();

                case "byWeekdays":
                    return new GeneratorXByWeekdays();

                case "byDateList":
                    return new GeneratorXDateList();

                case "tag":
                    return new GeneratorXTag();

                case "byOffset":
                    return new GeneratorXByOffset();

                default:
                    throw new NotImplementedException($"Unexpected generatorType '{scheduleType}'");
            }
        }
    }
}
