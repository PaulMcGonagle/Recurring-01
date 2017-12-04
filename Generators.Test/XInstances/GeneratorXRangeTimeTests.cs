using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Generators.XInstances;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Shouldly;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test.XInstances
{
    public class GeneratorXRangeTimeTests
    {
        public class XRangeExceptionTests
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
                            new XElement("rangeTime",
                                new XAttribute("start", "08:30"))),
                        typeof(KeyNotFoundException),
                        $"No Attribute found matching name 'period'"
                    },
                    {
                        new XDocument(
                            new XElement("rangeTime",
                                new XAttribute("start", "0830"))),
                        typeof(NodaTime.Text.UnparsableValueException),
                        null
                    },
                    {
                        new XDocument(
                            new XElement("rangeTime",
                                new XAttribute("start", "08:30"),
                                new XAttribute("end", "09:15"))),
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
                _generator = new GeneratorXRangeTime();
            }

            public void AndWhenCachesAreRetrieved()
            {
                _xElement.ExpandSource(out _caches);
            }

            public void AndWhenRangeTimeIsGenerated()
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
            private RangeTime _rangeTime;

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
                            new XElement("rangeTime",
                                new XAttribute("start", "09:20"),
                                new XAttribute("end", "10:03"))),
                        new LocalTime(09, 20),
                        new LocalTime(10, 03)
                    },
                    {
                        new XDocument(
                            new XElement("rangeTime",
                                new XAttribute("start", "12:15"),
                                new XAttribute("period", "PT45M"))),
                        new LocalTime(12, 15),
                        new LocalTime(12, 15).PlusMinutes(45)
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
                _generator = new GeneratorXRangeTime();
            }

            public void AndWhenCachesAreRetrieved()
            {
                _xElement.ExpandSource(out _caches);
            }

            public void AndWhenRangeTimeIsGenerated()
            {
                _vertex = _generator.Generate(_xElement, _caches);
            }

            public void ThenVertexIsARangeTime()
            {
                _vertex.GetType().ShouldBe(typeof(RangeTime));

                _rangeTime = (RangeTime)_vertex;
            }

            public void AndThenStartIsExpected(LocalTime expectedStart)
            {
                _rangeTime.Start.ShouldBe(expectedStart);
            }

            public void AndThenEndIsExpected(LocalTime expectedEnd)
            {
                _rangeTime.End.ShouldBe(expectedEnd);
            }
        }
    }
}
