using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

namespace ScheduleGeneration
{
    public static class Generate
    {
        public static void Go(string databaseName)
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
                var fakeClock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10));

//                var e = Event.Create
//                (
//                    schedule:
//                    ByWeekday.Create
//                    (
//                        clock: fakeClock,
//                        weekday: IsoDayOfWeek.Wednesday,
//                        range: new Range(2016, YearMonth.MonthValue.January, 01, 2016, YearMonth.MonthValue.January, 05)
//                    ),
//                    from: new LocalTime(16, 30),
//                    period: new PeriodBuilder {Minutes = 45}.Build(),
//                    timeZoneProvider: "Europe/London",
//                    location: new Location
//                    {
//                        Address = @"Flat 9
//26 Bryanston Square
//London
//W1H 2DS"
//                    }
//                );

//                // insert new document and creates 'Person' collection on the fly
//                db.Insert<Event>(e);

//                e.Title = "new title";

//                // partially updates person, only 'Age' attribute will be updated
//                await db.UpdateAsync<Event>(e);

//                // returns 27
//                string title = db.Query<Event>()
//                                  .Where(p => AQL.Contains(p.Title, "new title"))
//                                  .Select(p => p.Title)
//                                  .FirstOrDefault();

                /////////////////////// aql modification queries ////////////////////////////

                //
            }
        }
    }
}
