using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ArangoDB.Client;
using Generators;
using NodaTime;
using NodaTime.Testing;
using Scheduler;
using Scheduler.Generation;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;
using Scheduler.ScheduleInstances;
using Scheduler.Users;

namespace InitialiseDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator();

            Organisation organisation = new Organisation
            {
                Title = "Hampden Gurney Primary School",
                Location = new EdgeVertex<Location>(new Location
                {
                    Address = "Nutford Street",
                }),
            };

            var events = generator.GenerateEvents(
                sourceFile: "C:\\Users\\Paul\\Documents\\Sandbox\\Recurring\\Recurring 01\\Generators\\Sources\\HG.xml",
                organisation: organisation);

            var fakeClock = new FakeClock(Instant.FromUtc(2017, 04, 02, 03, 30, 00));

            using (var db = SchedulerDatabase.Database.Retrieve())
            {
                foreach (var @event in events)
                {
                    @event.Save(db, fakeClock);
                }
            }

            foreach (var @event in events)
            {
                GeneratedEvent.Generate(fakeClock, @event);
            }

            using (var db = SchedulerDatabase.Database.Retrieve())
            {
                foreach (var @event in events)
                {
                    @event.Save(db, fakeClock);
                }
            }

            var links = @events.SelectMany(e => e.GetLinks(4)).ToList();

            SortedDictionary<string, IVertex> cache = new SortedDictionary<string, IVertex>();

            foreach (var link in links)
            {
                if (!link.IsPersisted)
                    using (var db = SchedulerDatabase.Database.Retrieve())
                    {
                        link.Save(db, fakeClock);
                    }

                if (!cache.ContainsKey(link.Id))
                    cache.Add(link.Id, link);
            }

            ConsoleOutput.Output.DisplayList(
                events
                    .First()
                    .Serials
                    .SelectMany(s => s.ToVertex.Episodes)
                    .Select(e => e.To));
        }

        public static void Go(string databaseName)
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
                db.CreateCollection("Episode");
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

                new Profile
                {
                    Forename = "Paul",
                    Surname = "McGonagle",
                    Email = "paul@anemail.com",
                    HomeTimeZoneProvider = "Europe/London",
                    Organisations = new EdgeVertexs<IOrganisation>()
                }.Save(db, clock);

                new Profile
                {
                    Forename = "A",
                    Surname = "Dancer",
                    Email = "a.dancer@thestage.com",
                    HomeTimeZoneProvider = "Europe/Paris",
                }.Save(db, clock);

                var fakeClock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10));

                var e = Event.Create
                (
                    schedule:
                    ByWeekday.Create
                    (
                        clock: fakeClock,
                        weekday: IsoDayOfWeek.Wednesday,
                        range: new DateRange(2016, YearMonth.MonthValue.January, 01, 2016, YearMonth.MonthValue.January, 05)
                    ),
                    timerange: new TimeRange(new LocalTime(16, 30), new PeriodBuilder { Minutes = 45 }.Build()),
                    timeZoneProvider: "Europe/London",
                    location: new Location
                    {
                        Address = @"Flat 9
26 Bryanston Square
London
W1H 2DS"
                    }
                );

                try
                {
                    e.Save(db, clock);

                    GeneratedEvent.Generate(clock, e);

                    e.Serials.Save(db, fakeClock, e);
                
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
                        Serials = new EdgeVertexs<ISerial>(
                            toVertex: new Serial(
                                    schedule: new CompositeSchedule()
                                    {
                                        InclusionsEdges = new EdgeVertexs<ISchedule>
                                                {
                                                    new EdgeVertex<ISchedule>(new ByDayOfMonth
                                                        {
                                                            EdgeRange = new EdgeRange(TestData.DataRetrieval.Ranges["Schools.Term.201617.Winter"]),
                                                            Clock = new FakeClock(Instant.FromUtc(2016, 02, 10, 15, 40, 10)),
                                                            DayOfMonth = 10,
                                                        })
                                                    ,
                                                },
                                    },
                                    timeRange: new TimeRange(new LocalTime(16, 30), new PeriodBuilder { Minutes = 45 }.Build()),
                                    timeZoneProvider: "Europe/London")),
                        Location = new EdgeVertex<Location>(TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),
                    }.Save(db, clock);

                    new Event
                    {
                        Serials = new EdgeVertexs<ISerial>(
                            toVertex: new Serial(
                                    schedule: new CompositeSchedule()
                                    {
                                        InclusionsEdges = new EdgeVertexs<ISchedule>
                                        {
                                            new EdgeVertex<ISchedule>(new SingleDay
                                                {
                                                    Date = new Date(2000, YearMonth.MonthValue.January, 01),
                                                })
                                            ,
                                        },
                                    },
                                    timeRange: new TimeRange(new LocalTime(16, 30), new PeriodBuilder { Minutes = 45 }.Build()),
                                    timeZoneProvider: "Europe/London")),
                        Location = new EdgeVertex<Location>(TestData.DataRetrieval.Organisations["Lords Cricket Academy"].Location.ToVertex),
                    }.Save(db, clock);

                    new Event
                    {
                        Serials = new EdgeVertexs<ISerial>(
                            toVertex: new Serial(
                                schedule:new CompositeSchedule()
                                {
                                    InclusionsEdges = new EdgeVertexs<ISchedule>
                                    {
                                        new EdgeVertex<ISchedule>(new ByWeekdays
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
                                    },
                                timeRange: new TimeRange(new LocalTime(16, 30), new PeriodBuilder { Minutes = 45 }.Build()),
                                timeZoneProvider: "Europe/London")),
                    }.Save(db, clock);
                
                    new Event
                    {
                        Serials = new EdgeVertexs<ISerial>(
                            toVertex: new Serial(
                                    schedule: new CompositeSchedule()
                                    {
                                        InclusionsEdges = new EdgeVertexs<ISchedule>
                                        {
                                            new EdgeVertex<ISchedule>(new DateList
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
                                    },
                                    timeRange: new TimeRange(new LocalTime(16, 30), new PeriodBuilder { Minutes = 45 }.Build()),
                                    timeZoneProvider: "Europe/London")),
                    }.Save(db, clock);
                }
                catch (SaveException exception)
                {
                    Console.WriteLine(exception.Message);
                    throw;
                }
            }
        }
    }
}
