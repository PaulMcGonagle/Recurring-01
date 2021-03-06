﻿using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CoreLibrary;
using NodaTime;
using Scheduler;

namespace Generators.XInstances
{
    public class GeneratorXRangeDate : IGeneratorX
    {
        public IVertex Generate(XElement xRangeDate, IDictionary<string, IVertex> caches, IClock clock = null, string elementsName = null, string elementName = null)
        {
            Guard.AgainstNull(xRangeDate, nameof(xRangeDate));
            Guard.AgainstNull(caches, nameof(caches));

            var linkedRangeDate = UtilitiesLinks<RangeDate>
                .RetrieveAll(xRangeDate, caches)
                .SingleOrDefault();

            if (linkedRangeDate != null)
                return linkedRangeDate;

            var start = xRangeDate.RetrieveAttributeAsLocalDate("start");

            LocalDate to;

            if (xRangeDate.HasAttribute("end"))
            {
                to = xRangeDate.RetrieveAttributeAsLocalDate("end");
            }
            else
            {
                var duration = xRangeDate.RetrieveAttributeAsTimeSpan("duration");

                //todo change to Period
                to = start.PlusDays((int)duration.TotalDays);
            }
            var rangeDate = new RangeDate.Builder
            {
                Start = new Date(start),
                End = new Date(to)
            }.Build();

            rangeDate.Connect(xRangeDate.RetrieveTags(caches, elementsName));

            return rangeDate;
        }
    }
}
