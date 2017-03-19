using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using NodaTime;
using NodaTime.Text;
using Scheduler;
using Scheduler.Ranges;
using Scheduler.ScheduleEdges;

namespace Generators
{
    public static class Utilities
    {
        public static IEnumerable<XElement> RetrieveXReferences(XNode xElement, IEnumerable<XElement> xReferences, string type)
        {
            var paths = xReferences
                .Where(tr => tr.Attribute("type")?.Value == type)
                .Select(tr => tr.Attribute("path")?.Value)
                .Where(p => p != null);

            foreach (var path in paths)
            {
                var item = xElement.XPathSelectElement(path);

                if (item == null)
                    throw new Exception($"Unable to load reference {path}");

                if (item.Name != type)
                    throw new Exception($"Reference {path} expected type '{type}' but found '{item.Name}'");

                yield return item;
            }
        }

        public static DateRange RetrieveDateRange(XElement input, string elementName = "rangeDate")
        {
            var rangeDate = input.Element(elementName);

            if (rangeDate == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalDate(rangeDate, "start");
            var end = ParseAttributeAsLocalDate(rangeDate, "end");

            return new DateRange(
                from: new EdgeDate(start),
                to: new EdgeDate(end));
        }

        public static ITimeRange RetrieveTimeRange(XElement input, string elementName = "rangeTime")
        {
            var rangeTime = input.Element(elementName);

            if (rangeTime == null)
                throw new ArgumentException("Could not find Element {elementName}");

            var start = ParseAttributeAsLocalTime(rangeTime, "start");
            var end = ParseAttributeAsLocalTime(rangeTime, "end");

            var period = Period.Between(start, end, PeriodUnits.AllTimeUnits);

            return new TimeRange(
                from: start,
                period: period);
        }

        public static IList<XElement> RetrieveXTags(XElement input)
        {
            return input
                .Elements("tags")
                .Elements("tag")
                .ToList();
        }

        public static ITag RetrieveTag(XElement inputXTag)
        {
            var xTag = RetrieveXTags(inputXTag);

            var relatedTags = xTag.Select(RetrieveTag);

            var inputIdent = ParseAttribute(inputXTag, "id");
            var inputValue = ParseAttribute(inputXTag, "value");
            var inputPayload = inputXTag.Elements("payload").FirstOrDefault();

            var tag = new Tag(inputIdent.Value, inputValue.Value, inputPayload?.Value);

            tag.RelatedTags.AddRange(relatedTags.Select(relatedTag => new EdgeTag(relatedTag)));

            return tag;
        }

        public static IEnumerable<ITag> RetrieveTags(XElement input)
        {
            var xTags = RetrieveXTags(input);

            foreach (var xTag in xTags)
            {
                yield return RetrieveTag(xTag);
            }
        }

        public static LocalDate ParseAttributeAsLocalDate(XElement input, string name)
        {
            var attribute = input.Attribute(name);

            var localDatePattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd");
            var parseResult = localDatePattern.Parse(attribute?.Value);

            return parseResult.Value;
        }

        public static LocalTime ParseAttributeAsLocalTime(XElement input, string name)
        {
            var attribute = ParseAttribute(input, name);

            var localTimePattern = LocalTimePattern.CreateWithInvariantCulture("HH:mm");
            var parseResult = localTimePattern.Parse(attribute.Value);

            return parseResult.Value;
        }

        public static XAttribute ParseAttribute(XElement input, string name)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            var attribute = input.Attribute(name);

            if (attribute == null)
            {
                throw new ArgumentNullException(name);
            }

            return attribute;
        }
    }
}
