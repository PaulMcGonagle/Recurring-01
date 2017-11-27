using System.Collections.Generic;
using System.Xml.Linq;
using Scheduler.Persistance;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test
{
    public class UtilitiesTests
    {
        public class ExpandLinks
        {
            private XElement _xElement;
            private IDictionary<string, IVertex> _caches;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xElement"
                )
                {
                    new XElement("generator",
                        new XElement("caches",
                            new XElement("cache",
                                new XAttribute("name", "RangeDate.Year.2011"),
                                new XAttribute("type", "RangeDate"),
                                new XAttribute("path", "./generator/rangeDates/rangeDate[tags/tag[@id='name'][@value='Year.2011']]")
                                )
                            ),
                        new XElement("rangeDates",
                            new XElement("rangeDate",
                                new XElement("referenceValue",
                                    new XAttribute("attribute", "start"),
                                    new XAttribute("path", "./dates/date[tags/tag[@id='name'][@value='New Years Day 2017']]"),
                                    new XAttribute("node", "value")
                                    ),
                                new XAttribute("start", "1899-12-31"),
                                new XAttribute("end", "2016-04-01"),
                                new XElement("tags",
                                    new XElement("tag",
                                        new XAttribute("id", "name"),
                                        new XAttribute("value", "Year.2011")
                                        )
                                    )
                                )
                            ),
                        new XElement("dates",
                            new XElement("date",
                                new XAttribute("value", "2017-01-01"),
                                new XElement("tags",
                                    new XElement("tag",
                                        new XAttribute("id", "name"),
                                        new XAttribute("value", "New Years Day 2017")
                                        )
                                    )
                                )
                            )
                        ),
                }).BDDfy();
            }

            public void GivenADocument(XElement xElement)
            {
                _xElement = xElement;
            }

            public void WhenLinksAreExpanded()
            {
                _xElement.ExpandSource(out _caches);
            }
        }

        public class ExpandAttributeLinks
        {
            private XElement _xElement;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable(
                    "xElement"
                )
                {
                    new XElement("generator",
                        new XElement("rangeDates",
                            new XElement("rangeDate",
                                new XElement("referenceValue",
                                    new XAttribute("attribute", "start"),
                                    new XAttribute("path", "./dates/date[tags/tag/@id='name' and tags/tag/@value='New Years 2017']"),
                                    new XAttribute("node", "value")
                                ),
                                new XAttribute("start", "2016-03-05"),
                                new XAttribute("end", "2016-04-01")
                            )
                        ),
                        new XElement("dates",
                            new XElement("date",
                                new XAttribute("value", "2017-01-01"),
                                new XElement("tags",
                                    new XElement("tag",
                                        new XAttribute("id", "name"),
                                        new XAttribute("value", "New Years 2017")
                                    )
                                )
                            )
                        )
                    )
                }).BDDfy();
            }

            public void GivenADocument(XElement xElement)
            {
                _xElement = xElement;
            }

            public void WhenLinksAreExpanded()
            {
                _xElement.ExpandSource(out var _caches);
            }
        }
    }
}
