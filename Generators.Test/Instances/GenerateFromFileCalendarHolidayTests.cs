using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test.Instances
{
    public class GenerateFromFileCalendarHolidayTests
    {
        public class Go
        {
            private XElement _xElement;
            private IClock _clock;
            private string _tempFilename;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private ISchedule _schedule;
            private CompositeSchedule _compositeSchedule;
            private IEnumerable<IDate> _dates;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xElement",
                    "expectedDates"
                )
                {
                    {
                        new XElement("generator",
                            new XElement("calendars",
                                new XElement("calendar",
                                    new XElement("schedule",
                                        new XElement("inclusions",
                                            new XElement("byDateList",
                                                new XElement("date",
                                                    new XAttribute("value", "2016-12-25"),
                                                    new XElement("tags",
                                                        new XElement("tag",
                                                            new XAttribute("id", "name"),
                                                            new XAttribute("value", "Christmas Day 2016")
                                                        )
                                                    )
                                                ),
                                                new XElement("date",
                                                    new XAttribute("value", "2017-01-01"),
                                                    new XElement("tags",
                                                        new XElement("tag",
                                                            new XAttribute("id", "name"),
                                                            new XAttribute("value", "New Year's Day 2017")
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        ),
                        new List<LocalDate>()
                        {
                            new LocalDate(2016, 12, 25),
                            new LocalDate(2017, 01, 01)
                        }
                    }
                }).BDDfy();
            }

            public void GivenAnXElement(XElement xElement)
            {
                _xElement = xElement;
            }

            public void AndGivenAClock(IClock clock)
            {
                _clock = clock;
            }

            public void WhenDocumentIsSaved()
            {
                _tempFilename = System.IO.Path.GetTempFileName();

                _xElement.Save(_tempFilename);
            }

            public void AndWhenGeneratorIsRetrieved()
            {
                _generator = GeneratorFactory.Get("calendar");
            }

            public void AndWhenCalendarIsGenerated()
            {
                _vertexs = _generator
                    .Generate(_tempFilename, _clock)
                    .ToList();
            }

            public void AndWhenDocumentIsCleanedUp()
            {
                if (File.Exists(_tempFilename))
                {
                    File.Delete(_tempFilename);
                }
            }

            public void ThenAScheduleIsCreated()
            {
                _schedule = _vertexs
                    .OfType<ISchedule>()
                    .SingleOrDefault();

                _schedule.ShouldNotBeNull();
            }

            public void AndThenScheduleInstanceIsACompositeSchedule()
            {
                _schedule.ScheduleInstance.ShouldBeOfType<CompositeSchedule>();
                _compositeSchedule = (CompositeSchedule) _schedule.ScheduleInstance;
            }

            public void AndThenDateListIsNotEmpty()
            {
                _dates = _schedule.Generate(_clock);
            }

            public void AndThenDatesShouldBeExpected(IList<LocalDate> expectedDates)
            {
                _dates
                    .Select(date => date.Value)
                    .ShouldBe(expectedDates);
            }
        }
    }
}
