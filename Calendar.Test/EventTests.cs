using System.Linq;
using Shouldly;
using NodaTime;
using Scheduler;
using Scheduler.ScheduleInstances;
using TestStack.BDDfy;
using Xunit;

namespace Calendar.Test
{
    public class EventTests
    {
        public class CreateAndValidateEvent
        {
            private Calendar.Event _sut;
            private ISerial _serials;

            const string timeZoneProvider = "Europe/London";

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("sut", "expectedWeekday")
                    {
                        {
                            new Calendar.Event()
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
                                            },
                                            TimeZoneProvider = timeZoneProvider,
                                        }
                                    }
                                }
                            },
                            IsoDayOfWeek.Thursday
                        }
                    })
                    .BDDfy();
            }

            public void GivenSut(Calendar.Event sut)
            {
                _sut = sut;
            }

            public void WhenSomethingIsDone()
            {
                _serials = _sut.Serials;
            }

            public void ThenAllSerialsHaveTheCorrectWeekday(IsoDayOfWeek expectedWeekday)
            {
                _serials.Episodes().Select(e => e.From.DayOfWeek).ShouldAllBe(d => d.Equals(4));
            }
        }
    }
}
