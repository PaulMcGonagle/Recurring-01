using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Xml.Linq;
using Generators.XInstances;
using Generators.XScheduleInstances;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test.XInstances
{
    public class GeneratorXRangeDateTests
    {
        public class VerifyExceptionTests
        {
            private XDocument _xDocument;
            private IGeneratorX _generator;
            private IDictionary<string, IVertex> _caches;
            private XElement _xElement;
            private Exception _exception;

            [Fact]
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
                            new XElement("rangeDate",
                                new XAttribute("start", "2016-02-03"))),
                        typeof(KeyNotFoundException),
                        "No Attribute found matching name 'duration'"
                    },
                    {
                        new XDocument(
                            new XElement("rangeDate",
                                new XAttribute("start", "2016"))),
                        typeof(NodaTime.Text.UnparsableValueException),
                        null
                    },
                    {
                        new XDocument(
                            new XElement("rangeDate",
                                new XAttribute("start", "2016-10-01"),
                                new XAttribute("end", "2016-11-03"))),
                        null,
                        "Value cannot be null.\r\nParameter name: start"
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
                _xElement.ExpandSource(out _caches);
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
        public class XRangeValueTests
        {
            private XDocument _xDocument;
            private IGeneratorX _generator;
            private IDictionary<string, IVertex> _caches;
            private XElement _xElement;
            private IVertex _vertex;
            private RangeDate _rangeDate;

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
                            new XElement("rangeDate",
                                new XAttribute("start", "2016-10-03"),
                                new XAttribute("end", "2016-11-04"))),
                        new LocalDate(2016, 10, 03),
                        new LocalDate(2016, 11, 04)
                    },
                    {
                        new XDocument(
                            new XElement("rangeDate",
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
                _generator = new GeneratorXRangeDate();
            }

            public void AndWhenCachesAreRetrieved()
            {
                _xElement.ExpandSource(out _caches);
            }

            public void AndWhenRangeDateIsGenerated()
            {
                _vertex = _generator.Generate(_xElement, _caches);
            }

            public void ThenVertexIsARangeDate()
            {
                _vertex.GetType().ShouldBe(typeof(RangeDate));

                _rangeDate = (RangeDate) _vertex;
            }

            public void AndThenFromIsExpected(LocalDate expectedStart)
            {
                _rangeDate.Start.Date.Value.ShouldBe(expectedStart);
            }

            public void AndThenToIsExpected(LocalDate expectedEnd)
            {
                _rangeDate.End.Date.Value.ShouldBe(expectedEnd);
            }
        }
    }
}
