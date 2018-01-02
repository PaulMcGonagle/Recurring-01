using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;

namespace Generators.XScheduleInstances
{
    public class GeneratorXCompositeSchedule : IGeneratorXSchedule
    {
        public IVertex Generate(XElement xCompositeSchedule, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xCompositeSchedule, nameof(xCompositeSchedule));
            Guard.AgainstNull(caches, nameof(caches));

            if (xCompositeSchedule == null)
                throw new ArgumentNullException(nameof(xCompositeSchedule));

            var xScheduleAndExclusions = new List<Tuple<XElement, bool>>();

            {
                var inclusions = xCompositeSchedule
                    .Element("inclusions");

                if (inclusions != null)
                {
                    xScheduleAndExclusions
                        .AddRange(inclusions
                            .Elements()
                            .Select(xSchedule => new Tuple<XElement, bool>(xSchedule, false)));
                }
            }

            {
                var exclusions = xCompositeSchedule
                    .Element("exclusions");

                if (exclusions != null)
                {
                    xScheduleAndExclusions
                        .AddRange(exclusions
                            .Elements()
                            .Select(xSchedule => new Tuple<XElement, bool>(xSchedule, true)));
                }
            }

            var compositeSchedule = new CompositeSchedule();

            foreach (var xScheduleAndExclusion in xScheduleAndExclusions)
            {
                var xSchedule = xScheduleAndExclusion.Item1;
                var isExclusion = xScheduleAndExclusion.Item2;

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
                    elementsName: elementsName);

                schedule
                    .Connect(xSchedule
                        .RetrieveTags(caches, elementsName)
                        .ToList());

                if (isExclusion)
                {
                    compositeSchedule.Exclusions.Add(new EdgeSchedule((ISchedule)schedule));

               }
                else
                {
                    compositeSchedule.Inclusions.Add(new EdgeSchedule((ISchedule)schedule));
                }
            }

            var wrapperSchedule = new Schedule(compositeSchedule);

            wrapperSchedule
                .Connect(xCompositeSchedule
                    .RetrieveTags(caches, elementsName)
                    .ToList());

            return wrapperSchedule;
        }
    }
}
