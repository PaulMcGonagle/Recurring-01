using System;
using System.Collections.Generic;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Instances
{
    public class GenerateFromSchedule : IGenerateFromVertex
    {
        public IEnumerable<IVertex> Generate(IVertex vertex, IClock clock)
        {
            var schedule = vertex as ISchedule;

            if (schedule == null)
            {
                throw new Exception($"Expected ISchedule. Vertex was type {vertex.GetType()}");
            }

            yield break;
        }
    }
}
