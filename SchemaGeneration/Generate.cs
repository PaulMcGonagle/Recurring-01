using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.ScheduleInstances;

namespace SchemaGeneration
{
    public static class Generate
    {
        public static async Task Go(string databaseName)
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                s.Database = databaseName;
                s.Url = "http://localhost:8529";

                // you can set other settings if you need
                s.Credential = new NetworkCredential("root", "arango123");
                s.SystemDatabaseCredential = new NetworkCredential("root", "arango123");
            });

            using (var db = new ArangoDatabase(url: "http://localhost:8529", database: databaseName))
            {
                ///////////////////// insert and update documents /////////////////////////
                //db.CreateCollection("Event");

                var e = new MyCalendar.Event
                {
                    Location = "here",
                    Serials = new Serial
                    {
                        From = new LocalTime(16, 30),
                        Period = new PeriodBuilder { Minutes = 45 }.Build(),
                        TimeZoneProvider = "Europe/London",

                        Schedule = new CompositeSchedule()
                        {
                            Inclusions = new List<ISchedule>
                            {
                                new ByWeekday
                                {
                                    Range =
                                        new Range(2016, YearMonth.MonthValue.January, 01, 2016, YearMonth.MonthValue.January,
                                            05),
                                    Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                    Weekday = IsoDayOfWeek.Wednesday,
                                }
                            }
                        }
                    }
                };


                // insert new document and creates 'Person' collection on the fly
                db.Insert<MyCalendar.Event>(e);

                e.Title = "new title";

                // partially updates person, only 'Age' attribute will be updated
                await db.UpdateAsync<MyCalendar.Event>(e);

                // returns 27
                string location = db.Query<MyCalendar.Event>()
                                  .Where(p => AQL.Contains(p.Title, "new title"))
                                  .Select(p => p.Location)
                                  .FirstOrDefault();

                /////////////////////// aql modification queries ////////////////////////////

                //
            }
        }
    }
}
