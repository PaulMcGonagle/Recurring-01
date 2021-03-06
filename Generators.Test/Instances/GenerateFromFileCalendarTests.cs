﻿using System.Collections.Generic;
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
    public class GenerateFromFileCalendarTests
    {
        public class Go
        {
            private XElement _xElement;
            private IClock _clock;
            private string _tempFilename;
            private IGenerateFromFile _generator;
            private IEnumerable<IVertex> _vertexs;
            private ISchedule _schedule;
            private ByRangeDate _byRangeDate;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xElement"
                )
                {
                    new XElement("generator",
                        new XElement("calendars",
                            new XElement("calendar",
                                new XElement("schedule",
                                    new XElement("byRangeDate",
                                        new XAttribute("start", "2016-03-01"),
                                        new XAttribute("end", "2016-03-31")))))),

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
                _tempFilename = Path.GetTempFileName();

                _xElement.Save(_tempFilename);
            }

            public void AndWhenGeneratorIsRetrieved()
            {
                _generator = GenerateFromFileFactory.Get("calendar");
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

                _schedule
                    .ShouldNotBeNull();

                _schedule
                    .ScheduleInstance
                    .ShouldBeAssignableTo<ByRangeDate>();

                _byRangeDate = (ByRangeDate)_schedule
                    .ScheduleInstance;

                _byRangeDate
                    .ShouldNotBeNull();
            }
        }
    }
}
