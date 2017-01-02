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
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

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

                db.CreateCollection("Backup");
                db.CreateCollection("Event");
                db.CreateCollection("Profile");
                db.CreateCollection("Organisation");
                db.CreateCollection("Location");
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

                IClock clock = SystemClock.Instance;

                foreach (var date in TestData.DataRetrieval.Dates.Values)
                {
                    date.Save(db, clock);
                }

                foreach (var range in TestData.DataRetrieval.Ranges.Values)
                {
                    range.Save(db, clock);
                }

                foreach (var organisation in TestData.DataRetrieval.Organisations.Values)
                {
                    organisation.Save(db, clock);
                }

                new Scheduler.Users.Profile
                {
                    Forename = "Paul",
                    Surname = "McGonagle",
                    Email = "paul@anemail.com",
                    HomeTimeZoneProvider = "Europe/London",
                    Organisations = new EdgeVertexs<Organisation>()
                }.Save(db, clock);

                new Scheduler.Users.Profile
                {
                    Forename = "A",
                    Surname = "Dancer",
                    Email = "a.dancer@thestage.com",
                    HomeTimeZoneProvider = "Europe/Paris",
                }.Save(db, clock);
                
                var e = new Event
                {
                    Location = new EdgeVertex<Location>(TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),

                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",
                            
                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
                            {
                                InclusionsEdges = new EdgeVertexs<Schedule>
                                {
                                    new EdgeVertex<Schedule>(new ByDayOfMonth
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            DayOfMonth = 10,
                                        })
                                    ,
                                    new EdgeVertex<Schedule>(new SingleDay
                                        {
                                            Date = new Date(2000, YearMonth.MonthValue.January, 01),
                                        })
                                    ,
                                    new EdgeVertex<Schedule>(new ByDayOfYear
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            DayOfYear = 08,
                                        }
                                    ),
                                    new EdgeVertex<Schedule>(new ByWeekday
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            Weekday = IsoDayOfWeek.Wednesday,
                                        }
                                    ),
                                    new EdgeVertex<Schedule>(new ByWeekdays
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            Days = new List<IsoDayOfWeek>
                                            {
                                                IsoDayOfWeek.Saturday,
                                                IsoDayOfWeek.Sunday,
                                            }
                                        }
                                    ),
                                    new EdgeVertex<Schedule>(new DateList
                                        {
                                            Items = new List<Date>
                                            {
                                                new Date(2010, YearMonth.MonthValue.August, 09),
                                                new Date(2008, YearMonth.MonthValue.May, 30),
                                                new Date(1974, YearMonth.MonthValue.February, 09),
                                                new Date(1971, YearMonth.MonthValue.March, 15),
                                            }
                                        }
                                    ),
                                },
                            })
                        }
                    }
                };

                e.Save(db, clock);

                e.Title = "new title";

                // partially updates person, only 'Age' attribute will be updated
                e.Save(db, clock);
                
                // returns 27
                var entity = db
                    .Query<Event>()
                    .FirstOrDefault(p => AQL.Contains(p.Title, "new title"));

                entity?.SetToDelete();

                entity?.Save(db, clock);
                
                new Event
                {
                    Location = new EdgeVertex<Location>(TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),

                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",

                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
                            {
                                InclusionsEdges = new EdgeVertexs<Schedule>
                                {
                                    new EdgeVertex<Schedule>(new ByDayOfMonth
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            DayOfMonth = 10,
                                        })
                                    ,
                                },
                            })
                        }
                    }
                }.Save(db, clock);
                
                new Event
                {
                    Location = new EdgeVertex<Location>(TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),

                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",

                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
                            {
                                InclusionsEdges = new EdgeVertexs<Schedule>
                                {
                                    new EdgeVertex<Schedule>(new SingleDay
                                        {
                                            Date = new Date(2000, YearMonth.MonthValue.January, 01),
                                        })
                                    ,
                                },
                            })
                        }
                    }
                }.Save(db, clock);
                
                new Event
                {
                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",

                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
                            {
                                InclusionsEdges = new EdgeVertexs<Schedule>
                                {
                                    new EdgeVertex<Schedule>(new ByWeekdays
                                        {
                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                            Days = new List<IsoDayOfWeek>
                                            {
                                                IsoDayOfWeek.Saturday,
                                                IsoDayOfWeek.Sunday,
                                            }
                                        }
                                    ),                                },
                            })
                        }
                    }
                }.Save(db, clock);
                /*
                new Event
                {
                    Serials = new Serials
                    {
                        new Serial
                        {
                            From = new LocalTime(16, 30),
                            Period = new PeriodBuilder { Minutes = 45 }.Build(),
                            TimeZoneProvider = "Europe/London",

                            EdgeSchedule = new EdgeSchedule(new CompositeSchedule()
                            {
                                InclusionsEdges = new EdgeVertexs<Schedule>
                                {
                                    new EdgeVertex<Schedule>(new DateList
                                        {
                                            Items = new List<Date>
                                            {
                                                new Date(2010, YearMonth.MonthValue.August, 09),
                                                new Date(2008, YearMonth.MonthValue.May, 30),
                                                new Date(1974, YearMonth.MonthValue.February, 09),
                                                new Date(1971, YearMonth.MonthValue.March, 15),
                                            }
                                        }
                                    ),
                                },
                            })
                        }
                    }
                }.Save(db, clock);
                */
            }
        }
    }
}
