using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler;
using Scheduler.Persistance;
using Scheduler.ScheduleInstances;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test.XInstances
{
    public class GeneratorXByRangeDateTests
    {
        public class VerifyExceptionTests
        {
            private XDocument _xDocument;
            private IGeneratorX _generator;
            private IDictionary<string, IVertex> _caches;
            private XElement _xElement;
            private Exception _exception;

            //[Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xDocument",
                    "expectedExceptionType",
                    "expectedExceptionMessage"
                )
                {
                    {
                        new XDocument(
                            new XElement("byRangeDate",
                                new XAttribute("start", "2016-02-03"))),
                        typeof(ArgumentNullException),
                        "Value cannot be null.\r\nParameter name: duration"
                    },
                    {
                        new XDocument(
                            new XElement("byRangeDate",
                                new XAttribute("end", "2016-02-03"))),
                        typeof(ArgumentNullException),
                        "Value cannot be null.\r\nParameter name: start"
                    },
                    {
                        new XDocument(),
                        typeof(ArgumentNullException),
                        "Value cannot be null.\r\nParameter name: node"
                    },
                }).BDDfy();
            }

            public void GivenAnXmlString(XDocument xDocument)
            {
                _xDocument = xDocument;
            }

            public void WhenXElementIsRetrieved()
            {
                _xElement = _xDocument.Root;
            }

            public void WhenGeneratorIsLoaded()
            {
                _generator = new GeneratorXByRangeDate();
            }

            public void AndWhenCachesAreRetrieved()
            {
                _caches = _xElement.ExpandLinks();
            }

            public void AndWhenRangeDateIsGenerated()
            {
                _exception = Record.Exception(() =>
                {
                    _generator.Generate(_xElement, _caches);
                });
            }

            public void ThenExpectedExceptionIsThrown(Type expectedExceptionType)
            {
                if (expectedExceptionType == null)
                    return;

                _exception.ShouldNotBeNull();
                _exception.GetType().ShouldBe(expectedExceptionType);
            }

            public void AndThenExceptionHasCorrectMessage(string expectedExceptionMessage)
            {
                if (_exception == null
                    || expectedExceptionMessage == null)
                    return;

                _exception.Message.ShouldBe(expectedExceptionMessage);
            }
        }
        public class VerifyValueTests
        {
            private XDocument _xDocument;
            private IGeneratorX _generator;
            private IDictionary<string, IVertex> _caches;
            private XElement _xElement;
            private IVertex _vertex;

            private Schedule _schedule;
            private ByRangeDate _byRangeDate;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xDocument",
                    "expectedStart",
                    "expectedEnd"
                )
                {
                    {
                        new XDocument(
                            new XElement("byRangeDate",
                                new XAttribute("start", "2016-10-03"),
                                new XAttribute("end", "2016-11-04"))),
                        new LocalDate(2016, 10, 03),
                        new LocalDate(2016, 11, 04)
                    },
                    {
                        new XDocument(
                            new XElement("byRangeDate",
                                new XAttribute("start", "2016-10-03"),
                                new XAttribute("duration", "P5D"))),
                        new LocalDate(2016, 10, 03),
                        new LocalDate(2016, 10, 03).PlusDays(5)
                    },
                }).BDDfy();
            }

            public void GivenAnXmlString(XDocument xDocument)
            {
                _xDocument = xDocument;
            }

            public void WhenXElementIsRetrieved()
            {
                _xElement = _xDocument.Root;
            }

            public void WhenGeneratorIsLoaded()
            {
                _generator = new GeneratorXByRangeDate();
            }

            public void AndWhenCachesAreRetrieved()
            {
                _caches = _xElement.ExpandLinks();
            }

            public void AndWhenRangeDateIsGenerated()
            {
                _vertex = _generator.Generate(_xElement, _caches);
            }


            public void ThenVertexIsASchedule()
            {
                _vertex.GetType().ShouldBe(typeof(Schedule));

                _schedule = (Schedule)_vertex;
            }

            public void AndThenScheduleInstanceIsAByRangeDate()
            {
                _schedule.ScheduleInstance.ShouldBeOfType<ByRangeDate>();

                _byRangeDate = (ByRangeDate) _schedule.ScheduleInstance;
            }

            public void AndThenRangeIsValid()
            {
                _byRangeDate.EdgeRangeDate?.RangeDate.ShouldNotBeNull();
            }

            public void AndThenFromIsExpected(LocalDate expectedStart)
            {
                _byRangeDate.EdgeRangeDate.RangeDate.Start.Date.Value.ShouldBe(expectedStart);
            }

            public void AndThenToIsExpected(LocalDate expectedEnd)
            {
                _byRangeDate.EdgeRangeDate.RangeDate.End.Date.Value.ShouldBe(expectedEnd);
            }
        }
    }
}
