using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ArangoDB.Client;
using ArangoDB.Client.Data;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.ScheduleInstances;

namespace InitialiseDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Go("Scheduling");

            if (a.Status != TaskStatus.RanToCompletion)
            {
                if (a.Exception != null)
                    throw a.Exception;
            }

            Console.ReadKey();
        }

        public static async Task Go(string databaseName)
        {
            ArangoDatabase.ChangeSetting(s =>
            {
                Console.WriteLine("ChangeSetting");
                s.Url = "http://localhost:8529";
                s.Database = databaseName;

                // you can set other settings if you need
                s.Credential = new NetworkCredential("root", "arango123");
                s.SystemDatabaseCredential = new NetworkCredential("root", "arango123");
            });
            
            using (var db = ArangoDatabase.CreateWithSetting())
            {
                var collectionNames = db.ListCollections().Select(s => s.Name);

                if (!collectionNames.Contains($"Event"))
                    db.CreateCollection("Event");

                var e = new Scheduler.Event
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

                e.Save(db);

                e.Title = "new title";

                // partially updates person, only 'Age' attribute will be updated
                e.Save(db);

                // returns 27
                string location = db.Query<Event>()
                                  .Where(p => AQL.Contains(p.Title, "new title"))
                                  .Select(p => p.Location)
                                  .FirstOrDefault();

                /////////////////////// aql modification queries ////////////////////////////

                //
            }
        }
    }
}
