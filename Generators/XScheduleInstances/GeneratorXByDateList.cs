using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.XScheduleInstances
{
    public class GeneratorXDateList : IGeneratorX
    {
        public IVertex Generate(XElement xDateList, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xDateList, nameof(xDateList));
            Guard.AgainstNull(caches, nameof(caches));

            var xDates = xDateList
                .Elements(elementsName ?? "dates")
                .SingleOrDefault()
            ?? throw new Exception("Missing dates");

            var dates = xDates
                .RetrieveDates(clock, caches, elementName ?? "date")
                .ToList();

            var dateList = new Schedule(
                new ByDateList.Builder
                    {
                        Items = new EdgeVertexs<IDate>(dates)
                    }.Build());

            dateList.Connect(xDateList.RetrieveTags(caches, elementsName));

            return dateList;
        }
    }
}
