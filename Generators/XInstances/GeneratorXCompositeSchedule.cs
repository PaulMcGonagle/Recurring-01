using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;

namespace Generators.XInstances
{
    public class GeneratorXCompositeSchedule : IGeneratorXSchedule
    {
        public IVertex Generate(XElement xSchedules, IDictionary<string, IVertex> caches, string elementsName = null)
        {
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

                    default:
                        throw new Exception($"Unable to generate schedule from type {type}");
                }

                var schedule = generatorX.Generate(
                    xSchedule,
                    caches,
                    elementsName);

                var t = xSchedule.RetrieveTags(caches, elementsName).ToList();
                schedule.Connect(t);

                compositeSchedule.InclusionsEdges.Add(new EdgeSchedule((ISchedule)schedule));
            }

            return compositeSchedule;
        }
    }
}
