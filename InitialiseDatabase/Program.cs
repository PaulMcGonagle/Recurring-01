using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArangoDB.Client;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Persistance;
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

                foreach (var collectionName in collectionNames)
                {
                    db.DropCollection(collectionName);
                }

                db.CreateCollection("Event");
                db.CreateCollection("Profile");
                db.CreateCollection("Organisation");
                db.CreateCollection("Range");
                db.CreateCollection("Date");
                db.CreateCollection("Serial");
                db.CreateCollection("CompositeSchedule");
                db.CreateCollection("ByDayOfMonth");
                db.CreateCollection("ByDayOfYear");
                db.CreateCollection("ByWeekday");
                db.CreateCollection("ByWeekdays");
                db.CreateCollection("DateList");
                db.CreateCollection("SingleDay");
                db.CreateCollection("Edge", type: CollectionType.Edge);

                foreach (var date in TestData.DataRetrieval.Dates.Values)
                {
                    date.Save(db);
                }

                foreach (var range in TestData.DataRetrieval.Ranges.Values)
                {
                    range.Save(db);
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

                var e = new Event
                {
                    Location = "here",
                    
                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",

                            Schedule = new CompositeSchedule()
                            {
                                InclusionsEdges = new Edges
                                {
                                    new Edge
                                    {
                                        ToVertex = new ByDayOfMonth
                                        {
                                            Range = TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"],
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            DayOfMonth = 10,
                                        }
                                    },
                                    new Edge
                                    {
                                        ToVertex = new SingleDay
                                        {
                                            Date = new Date(2000, YearMonth.MonthValue.January, 01),
                                        }
                                    },
                                    new Edge
                                    {
                                        ToVertex = new ByDayOfYear
                                        {
                                            Range = TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"],
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            DayOfYear = 08,
                                        }
                                    },
                                    new Edge
                                    {
                                        ToVertex = new ByWeekday
                                        {
                                            Range = TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"],
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            Weekday = IsoDayOfWeek.Wednesday,
                                        }
                                    },
                                    new Edge
                                    {
                                        ToVertex = new ByWeekdays
                                        {
                                            Range = TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"],
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            Days = new List<IsoDayOfWeek>
                                            {
                                                IsoDayOfWeek.Saturday,
                                                IsoDayOfWeek.Sunday,
                                            }
                                        }
                                    },
                                    new Edge
                                    {
                                        ToVertex = new DateList
                                        {
                                            Items = new List<Date>
                                            {
                                                new Date(2010, YearMonth.MonthValue.August, 09),
                                                new Date(2008, YearMonth.MonthValue.May, 30),
                                                new Date(1974, YearMonth.MonthValue.February, 09),
                                                new Date(1971, YearMonth.MonthValue.March, 15),
                                            }
                                        }
                                    },
                                },
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
