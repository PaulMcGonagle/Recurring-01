using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Generators;
using Generators.Instances;
using NodaTime;
using Scheduler;
using Scheduler.Ranges;

namespace SourceScenarios.Holidays
{
    public class Generator : SourceScenario
    {
        public Generator(IClock clock)
        {
            Clock = clock;
        }

        public IEnumerable<IRangeDate> Years
        {
            get
            {
                var years = new GenerateFromFileRangeDates()
                    .Generate("C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Holidays\\Files\\Years.xml",
                        Clock)
                    .OfType<IRangeDate>()
                    .ToList();

                return years;
            }
        }

        public IEnumerable<IDate> HolidayDates
        {
            get
            {
                var csvData = File.ReadAllText("C:\\Users\\mcgon\\Source\\Repos\\Recurring-01\\Holidays\\Files\\Holidays.csv");

                var isHeaderRow = true;

                var rows = csvData.Split(
                    new[]
                    {
                        Environment.NewLine
                    },
                    StringSplitOptions.None
                );

                //Execute a loop over the rows.  
                foreach (var row in rows)
                {
                    if (isHeaderRow)
                    {
                        isHeaderRow = false;
                        continue;
                    }

                    if (string.IsNullOrEmpty(row))
                        continue;

                    //Execute a loop over the columns.  
                    var cells = row.Split(',');

                    var dateText = cells[0];
                    var name = cells[1];
                    var year = cells[2];

                    var dateValue = Retriever.RetrieveLocalDate(dateText);

                    var date = new Date(dateValue);

                    date.Connect("year", year);
                    date.Connect("name", name);

                    yield return date;
                }
            }
        }
    }
}
