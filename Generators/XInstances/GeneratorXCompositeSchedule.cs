using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Generators.XInstances
{
    public class GeneratorXCompositeSchedule : IGeneratorXSchedule
    {
        public IVertex Generate(XElement xSchedules, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            if (xSchedules == null)
                throw new ArgumentNullException(nameof(xSchedules));

            var compositeSchedule = new CompositeSchedule();

            foreach (var xSchedule in xSchedules.Elements())
            {
                var type = xSchedule.Name.LocalName;

                if (string.IsNullOrWhiteSpace(type))
                    throw new Exception("Could not determine schedule type");

                IGeneratorX generatorX;

                switch (type)
                {
                    case "byOffset":
                        generatorX = new GeneratorXByOffset();
                        break;

                    case "byWeekdays":
                        generatorX = new GeneratorXByWeekdays();
                        break;

                    case "byRangeDate":
                        generatorX = new GeneratorXByRangeDate();
                        break;

                    case "byDateList":
                        generatorX = new GeneratorXDateList();
                        elementsName = type;
                        break;

                    default:
                        throw new Exception($"Unable to generate schedule from type {type}");
                }

                var schedule = generatorX.Generate(
                    xSchedule,
                    caches,
                    elementsName);

                var tags = xSchedule
                    .RetrieveTags(caches, elementsName)
                    .ToList();

                schedule.Connect(tags);

                compositeSchedule.Inclusions.Add(new EdgeSchedule((ISchedule)schedule));
            }

            return compositeSchedule;
        }
    }
}
