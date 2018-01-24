using System;
using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXTerm : IGeneratorX
    {
        public IVertex Generate(XElement xTerm, IDictionary<string, IVertex> caches, IClock clock = null,
            string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xTerm, nameof(xTerm));
            Guard.AgainstNull(caches, nameof(caches));

            var xTermSchedule = xTerm
                                    .Element("schedule")
                                    ?.Elements()
                                    .SingleOrDefault()
                                ?? throw new Exception($"Missing term schedule");

            var termSchedule = GenerateFromFileFactory
                                   .GetXSchedule(xTermSchedule)
                                   .Generate(xTermSchedule, caches, clock) as ISchedule
                               ?? throw new Exception("Could not generate termSchedule");

            var termBreakVertexs = xTerm
                .Elements("breaks")
                .Elements("schedule")
                .Select(xBreakSchedule => xBreakSchedule.Elements().SingleOrDefault())
                .Select(xBreakScheduleInstance => GenerateFromFileFactory
                    .GetXSchedule(xBreakScheduleInstance)
                    .Generate(xBreakScheduleInstance, caches, clock))
                .ToList();

            if (!termBreakVertexs.TrueForAll(v => v is ISchedule))
                throw new Exception("Generated vertex not an ISchedule");

            var termBreaks = termBreakVertexs.ConvertAll(v => (ISchedule) v);

            var compositeSchedule = new CompositeSchedule.Builder
            {
                Inclusion = termSchedule,
                Exclusions = new EdgeVertexs<ISchedule>(termBreaks),
            }.Build();

            var schedule = new Schedule(compositeSchedule)
            {
                Tags = new EdgeVertexs<ITag>(xTerm.RetrieveTags(caches)),
            };

            return schedule;
        }
    }
}