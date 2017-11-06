﻿using System.Xml.Linq;
using TestStack.BDDfy;
using Xunit;

namespace Generators.Test
{
    public class UtilitiesTests
    {
        public class ExpandLinks
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
                        new XElement("caches",
                            new XElement("cache",
                                new XAttribute("name", "RangeDate.Year.2011"),
                                new XAttribute("type", "RangeDate"),
                                new XAttribute("path", "./generator/rangeDates/rangeDate[tags/tag[@id='name'][starts-with(@value,'Year.2011')]]")
                                )
                            ),
                        new XElement("rangeDates",
                            new XElement("rangeDate",
                                new XAttribute("start", "2016-03-05"),
                                new XAttribute("end", "2016-04-01"),
                                new XElement("tags",
                                    new XElement("tag",
                                        new XAttribute("id", "name"),
                                        new XAttribute("value", "Year.2011")
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
                var t = _xElement.ExpandLinks();
            }
        }
    }
}