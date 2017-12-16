using System.Linq;
using Shouldly;
using NodaTime;
using Scheduler;
using Scheduler.Calendars;
using Scheduler.Persistance;
using Scheduler.Ranges;
using Scheduler.ScheduleInstances;
using Scheduler.Test;
using Scheduler.Users;
using TestStack.BDDfy;
using Xunit;
using Event = Scheduler.Event;

namespace Calendar.Test
{
    public class EventTests
    {
        public class CreateAndValidateEvent
        {
            private IEvent _sut;
            private IEdgeVertexs<ISerial> _serials;
            private IClock _clock;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "clock", "expectedWeekday", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event.Builder
                            {
                                Serial = new Serial.Builder
                                    {
                                        Schedule = new Schedule(new ByWeekdays.Builder
                                            {
                                                Weekdays = new []{ IsoDayOfWeek.Thursday, },
                                                RangeDate = new RangeDate.Builder
                                                {
                                                    Start = new Date(2016, YearMonth.MonthValue.January, 01),
                                                    End = new Date(2016, YearMonth.MonthValue.March, 31),
                                                }.Build(),
                                            }.Build()),
                                        RangeTime = new RangeTime.Builder
                                            {
                                                Start = new LocalTime(16, 45),
                                                Period = new PeriodBuilder { Minutes = 45 }.Build(),
                                            }.Build(),
                                        TimeZoneProvider = TimeZoneProvider,
                                    }.Build(),
                                Title = "Street dance",
                                Location = new Location(),
                                Instance = new Instance()
                            }.Build(),
                            fakeClock,
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
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.Start.DayOfWeek)
                    .ShouldAllBe(d => d.Equals((int)expectedWeekday));
            }

            public void AndThenAllStartTimesAreCorrect(LocalTime expectedStartTime)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.Start.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void AndThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.End.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedEndTime));
            }
        }
        public class VerifyTimeInOtherTimeZone
        {
            private IEvent _sut;
            private IClock _clock;
            private IEdgeVertexs<ISerial> _serials;

            private const string TimeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                var fakeClock = ScheduleTestHelper.GetFakeClock(2016, 05, 01);

                this.WithExamples(new ExampleTable("sut", "clock", "expectedStartTime", "expectedEndTime")
                    {
                        {
                            new Event
                            {
                                Serials = new EdgeVertexs<ISerial>(new Serial.Builder
                                {
                                    Schedule = new Schedule(new SingleDay.Builder
                                            {
                                                Date = new Scheduler.Date(2016, YearMonth.MonthValue.July, 01),
                                            }.Build()),
                                    RangeTime = new RangeTime.Builder
                                        {
                                            Start = new LocalTime(14, 00),
                                            Period = new PeriodBuilder { Minutes = 1 }.Build()
                                        }.Build(),
                                    TimeZoneProvider = "Europe/London",
                                }.Build()),
                                Title = "Street dance",
                            },
                            fakeClock,
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
                _serials.SelectMany(s => s.ToVertex.GenerateEpisodes(_clock))
                    .Select(e => e.Start.TimeOfDay)
                    .ShouldAllBe(d => d.Equals(expectedStartTime));
            }

            public void AndThenAllEndTimesAreCorrect(LocalTime expectedEndTime)
            {
            }
        }
    }
}
