﻿using System.Collections.Generic;
using NodaTime;
using Scheduler.Persistance;
using Scheduler.ScheduleEdges;
using Shouldly;
using TestStack.BDDfy;
using Scheduler.ScheduleInstances;
using Xunit;

namespace Scheduler.Test
{
    public class SerialTests
    {
        public class VerifyDateOutOfBoundsExceptionIsThrown
        {
            private Serial _sut;
            private IEpisodes _episodes;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "expectedEpisodes")
                    {
                        {
                            new Serial(new DateList
                                    {
                                        Items = new List<Date>()
                                        {
                                            new Date(2016, YearMonth.MonthValue.January, 05),
                                            new Date(2016, YearMonth.MonthValue.January, 06),
                                            new Date(2016, YearMonth.MonthValue.January, 07),
                                        }
                                    })
                            {
                                From = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Hours = 00, Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            new Episodes
                            {
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 05), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build(),

                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 06), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                                new Episode
                                {
                                    From = DateTimeHelper.GetZonedDateTime(new Date(2016, YearMonth.MonthValue.January, 07), new LocalTime(15, 30), timeZoneProvider),
                                    Period = new PeriodBuilder {Hours = 00, Minutes = 30}.Build()
                                },
                            }
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(Serial sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _episodes = _sut.Episodes;
            }

            public void ThenEpisodesAreExpected(IEpisodes expectedEpisodes)
            {
                _episodes.ShouldBe(expectedEpisodes);
            }
        }

        public class VerifyMissingPropertyThrowsArgumentException
        {
            private Serial _sut;
            private IEpisodes _episodes;
            private System.Exception _exception;

            [Fact]
            public void Execute()
            {
                const string timeZoneProvider = "Europe/London";

                this.WithExamples(new ExampleTable("sut", "parameterName")
                    {
                        {
                            new Serial(new DateList { Items = new List<Date>(), })
                            {
                                Period = new PeriodBuilder {Minutes = 30,}.Build(),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "From"
                        },
                        {
                            new Serial(new DateList { Items = new List<Date>(), })
                            {
                                From = new LocalTime(15, 30),
                                TimeZoneProvider = timeZoneProvider,
                            },
                            "Period"
                        },
                        {
                            new Serial(new DateList { Items = new List<Date>(), })
                            {
                                From = new LocalTime(15, 30),
                                Period = new PeriodBuilder {Minutes = 30,}.Build(),
                            },
                            "TimeZoneProvider"
                        },
                    })
                    .BDDfy();
            }

            public void GivenACalendarEvent(Serial sut)
            {
                _sut = sut;
            }

            public void WhenEpisodesAreRetrieved()
            {
                _exception = Record.Exception(() => { _episodes = _sut.Episodes; });
            }

            public void ThenArgumentExceptionIsThrown(string parameterName)
            {
                _exception.ShouldNotBeNull();

                _exception.Message.ShouldBe(parameterName);
            }
        }
    }
}