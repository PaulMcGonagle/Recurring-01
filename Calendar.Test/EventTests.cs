using System.Linq;
using Shouldly;
using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using Scheduler.Test;
using TestStack.BDDfy;
using Xunit;

namespace Calendar.Test
{
    public class EventTests
    {
        public class CreateAndValidateEvent
        {
            private Event _sut;
            private ISerial _serials;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "expectedWeekday", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event()
                            {
                                Title = "Street dance",
                                Serials = new Serials
                                {
                                    {
                                        new Serial
                                        {
                                            TimeStart = new LocalTime(16, 45),
                                            Period = new PeriodBuilder { Minutes = 45}.Build(),
                                            Schedule = new ByWeekday
                                            {
                                                DateFrom = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.September, 22),
                                                DateTo = DateTimeHelper.GetLocalDate(2016, YearMonth.MonthValue.December, 20),
                                                Weekday = IsoDayOfWeek.Thursday,
                                                Clock = fakeClock,
                                            },
                                            TimeZoneProvider = TimeZoneProvider,
                                        }
                                    }
                                },
                            },
                            IsoDayOfWeek.Thursday,
                            new LocalTime(16, 45),
                            new LocalTime(17, 30)
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(Event sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {
                _serials = _sut.Serials;
            }

            public void ThenAllSerialsHaveTheCorrectWeekday(IsoDayOfWeek expectedWeekday)
            {
                _serials.Episodes.Select(e => e.From.DayOfWeek).ShouldAllBe(d => d.Equals((int)expectedWeekday));
            }

            public void ThenAllStartTimesAresCorrect(LocalTime expectedStartTime)
            {
                _serials.Episodes.Select(e => e.From.TimeOfDay).ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void ThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
                _serials.Episodes.Select(e => e.End.TimeOfDay).ShouldAllBe(d => d.Equals(expectedEndTime));
            }
        }
        public class VerifyTimeInOtherTimeZone
        {
            private Event _sut;
            private ISerial _serials;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event()
                            {
                                Title = "Street dance",
                                Serials = new Serials
                                {
                                    new Serial
                                    {
                                        Schedule = 
                                            new SingleDay
                                            {
                                                Date = new LocalDate(2016, 07, 01),
                                            },
                                        TimeStart = new LocalTime(14, 00),
                                        Period = new PeriodBuilder { Minutes = 1 }.Build(),
                                        TimeZoneProvider = "Europe/London",
                                    }
                                },
                            },
                            new LocalTime(14, 00),
                            new LocalTime(14, 45)
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(Event sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {
                _serials = _sut.Serials;
            }

            public void ThenAllStartTimesAreCorrect(LocalTime expectedStartTime)
            {
                _serials.Episodes.Select(e => e.From.TimeOfDay).ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void ThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
            }
        }
    }
}
