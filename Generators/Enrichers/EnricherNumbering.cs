using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scheduler;

namespace Generators.Enrichers
{
    public class EnricherNumbering
    {
        public void Go(
            IEnumerable<IDate> dates,
            string ident,
            int firstNumber = 0, 
            int increment = 1,
            string groupTag = null,
            ITag endTag = null)
        {
            var iter = firstNumber;

            foreach (var date in dates)
            {
                var value = iter.ToString();

                var tag = date.Connect(ident: ident, value: value);

                tag.Connect(endTag);

                iter += increment;
            }
        }
    }
}
