using System;
using Scheduler.Persistance;
using Scheduler.Ranges;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Generators.XInstances
{
    public class GeneratorXDateRange : IGeneratorX
    {
        public bool TryGenerate(XElement xRangeDate, IDictionary<string, IVertex> caches, out IVertex vertex, string elementsName = null)
        {
            var from = xRangeDate.RetrieveAttributeAsLocalDate("start");
            var to = xRangeDate.RetrieveAttributeAsLocalDate("end");

            vertex = new DateRange(from, to);

            try
            {
                vertex.Connect(xRangeDate.RetrieveTags(caches, elementsName));
            }
            catch (Exception)
            {
                //todo Check exception type

                return false;
            }

            return true;
        }
    }
}
