using System.Collections.Generic;
using Scheduler;
using Scheduler.Persistance;

namespace Generators.Enrichers
{
    public class EnricherNumbering
    {
        public void Go(
            IEnumerable<IVertex> vertexs,
            string ident,
            int firstNumber = 0, 
            int increment = 1,
            string groupTag = null,
            ITag endTag = null)
        {
            var iter = firstNumber;

            foreach (var vertex in vertexs)
            {
                var value = iter.ToString();

                var tag = vertex.Connect(ident: ident, value: value);

                tag.Connect(endTag);

                iter += increment;
            }
        }
    }
}
