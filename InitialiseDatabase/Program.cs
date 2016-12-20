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
                var collectionNames = db.ListCollections().Select(s => s.Name).ToArray();

                if (collectionNames.Contains($"Event"))
                {
                    db.DropCollection("Event");
                }

                db.CreateCollection("Event");

                if (collectionNames.Contains($"Profile"))
                {
                    db.DropCollection("Profile");
                }

                db.CreateCollection("Profile");

                if (collectionNames.Contains($"Organisation"))
                {
                    db.DropCollection("Organisation");
                }

                db.CreateCollection("Organisation");

                if (collectionNames.Contains($"Range"))
                {
                    db.DropCollection("Range");
                }

                db.CreateCollection("Range");

                foreach (var range in TestData.DataRetrieval.Ranges.Values)
                {
                    range.Save(db);
                }

                foreach (var date in TestData.DataRetrieval.Dates)
                {
                    date.Value.Save(db);
                }

                new Scheduler.Users.Profile
                {
                    Forename = "Paul",
                    Surname = "McGonagle",
                    Email = "paul@anemail.com",
                    TimeZoneProvider = "Europe/London",
                }.Save(db);

                new Scheduler.Users.Profile
                {
                    Forename = "A",
                    Surname = "Dancer",
                    Email = "a.dancer@thestage.com",
                    TimeZoneProvider = "Europe/Paris",
                }.Save(db);

                new Scheduler.Users.Organisation
                {
                    Title = "Hampden Gurney Primary School",
                }.Save(db);

                new Scheduler.Users.Organisation
                {
                    Title = "Sylvia Young Theatre School",
                }.Save(db);

                new Scheduler.Users.Organisation
                {
                    Title = "Lord Cricket Academy",
                }.Save(db);

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
                                    Range = TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter.Start"],
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
                var entity = db
                    .Query<Event>()
                    .FirstOrDefault(p => AQL.Contains(p.Title, "new title"));

                entity?.SetToDelete();

                entity?.Save(db);

                /////////////////////// aql modification queries ////////////////////////////

                
                //
            }
        }
    }
}
