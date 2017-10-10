using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;

namespace Generators.XInstances
{
    public class GeneratorXDateList : IGeneratorX
    {
        public IVertex Generate(XElement xDateList, IDictionary<string, IVertex> caches, string elementsName = null, IClock clock = null)
        {
            var dates = xDateList
                .RetrieveDates(clock, caches, elementsName)
                .ToList();

            var dateList = DateList
                .Create(dates);

            dateList.Connect(xDateList.RetrieveTags(caches, elementsName));

            return dateList;
        }
    }
}
