using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generators;
using Generators.Instances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.Ranges;

namespace Holidays
{
    public class Generate : SourceScenario
    {
        public Generate(IClock clock)
        {
            Clock = clock;
        }

        public Generate WithYears()
        {
            var holidays = new GenerateFromFileSchedule()
                .Generate("C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Holidays\\Files\\Holidays.xml",
                    Clock)
                .OfType<IRangeDate>()
                .ToList();

            Vertexs = Vertexs.Union(holidays);

            return this;
        }


    }
}
