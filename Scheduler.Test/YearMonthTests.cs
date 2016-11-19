using Shouldly;
using System.Linq;
using TestStack.BDDfy;
using Xunit;

namespace Scheduler.Test
{
    public class YearMonthTest
    {
        public class VerifyMonthCountSetter
        {
            private int _monthCount;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("MonthCount", "ExpectedIsExceptionThrown")
                    {
                        {0, true},
                        {1, true},
                        {(YearMonth.YearLimit.Lower*12) - 1, true},
                        {(YearMonth.YearLimit.Lower*12), false},
                        {(YearMonth.YearLimit.Upper*12) - 1, false},
                        {(YearMonth.YearLimit.Upper*12), false},
                    })
                    .BDDfy();
            }

            public void GivenAMonthCount(int monthCount)
            {
                _monthCount = monthCount;
            }

            public void ThenAnExceptionIsThrownAsExpected(bool expectedIsExceptionThrown)
            {
                if (expectedIsExceptionThrown)
                    Assert.Throws<System.ArgumentOutOfRangeException>(() => new YearMonth {MonthCount = _monthCount});
            }
        }

        public class VerifyMonthCountReturnsIdenticalValue
        {
            private int _sut;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("SUT", "Expected Year", "Expected Month")
                    {
                        {(YearMonth.YearLimit.Lower*12) + 00, YearMonth.YearLimit.Lower, YearMonth.MonthValue.January},
                        {(YearMonth.YearLimit.Lower*12) + 11, YearMonth.YearLimit.Lower, YearMonth.MonthValue.December},
                        {(YearMonth.YearLimit.Upper*12) + 11, YearMonth.YearLimit.Upper, YearMonth.MonthValue.December},
                        {(YearMonth.YearLimit.Upper*12) + 11, YearMonth.YearLimit.Upper, YearMonth.MonthValue.December},
                    })
                    .BDDfy();
            }

            public void GivenAMonthCount(int sut)
            {
                _sut = sut;
            }

            public void ThenMonthCountShouldBeSameAsSut()
            {
                var yearMonth = new YearMonth {MonthCount = _sut};

                yearMonth.MonthCount.ShouldBe(_sut);
            }

            public void AndThenYearIsValid(int expectedYear)
            {
                var yearMonth = new YearMonth {MonthCount = _sut};

                yearMonth.Year.ShouldBe(expectedYear);
            }

            public void AndThenMonthIsValid(YearMonth.MonthValue expectedMonth)
            {
                var yearMonth = new YearMonth {MonthCount = _sut};

                yearMonth.Month.ShouldBe(expectedMonth);
            }
        }

        public class VerifyAYearMonthIsCreatedFromAMonthCount
        {
            private int _sut;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("monthCount", "Expected Year", "Expected Month")
                    {
                        {(1000*12) + 00, 1000, YearMonth.MonthValue.January},
                        {(1000*12) + 01, 1000, YearMonth.MonthValue.February},
                        {(1000*12) + 02, 1000, YearMonth.MonthValue.March},
                        {(1000*12) + 03, 1000, YearMonth.MonthValue.April},
                        {(1000*12) + 04, 1000, YearMonth.MonthValue.May},
                        {(1000*12) + 05, 1000, YearMonth.MonthValue.June},
                        {(1000*12) + 06, 1000, YearMonth.MonthValue.July},
                        {(1000*12) + 07, 1000, YearMonth.MonthValue.August},
                        {(1000*12) + 08, 1000, YearMonth.MonthValue.September},
                        {(1000*12) + 09, 1000, YearMonth.MonthValue.October},
                        {(1000*12) + 10, 1000, YearMonth.MonthValue.November},
                        {(1000*12) + 11, 1000, YearMonth.MonthValue.December},
                        {(9999*12) + 00, 9999, YearMonth.MonthValue.January},
                        {(9999*12) + 01, 9999, YearMonth.MonthValue.February},
                        {(9999*12) + 02, 9999, YearMonth.MonthValue.March},
                        {(9999*12) + 03, 9999, YearMonth.MonthValue.April},
                        {(9999*12) + 04, 9999, YearMonth.MonthValue.May},
                        {(9999*12) + 05, 9999, YearMonth.MonthValue.June},
                        {(9999*12) + 06, 9999, YearMonth.MonthValue.July},
                        {(9999*12) + 07, 9999, YearMonth.MonthValue.August},
                        {(9999*12) + 08, 9999, YearMonth.MonthValue.September},
                        {(9999*12) + 09, 9999, YearMonth.MonthValue.October},
                        {(9999*12) + 10, 9999, YearMonth.MonthValue.November},
                        {(9999*12) + 11, 9999, YearMonth.MonthValue.December},
                    })
                    .BDDfy();
            }

            public void GivenAMonthCount(int monthCount)
            {
                _sut = monthCount;
            }

            public void ThenTheYearIsValid(int expectedYear, int expectedMonth)
            {
                var yearMonth = new YearMonth {MonthCount = _sut,};

                yearMonth.Year.ShouldBe(expectedYear);
            }

            public void AndThenTheMonthIsValid(YearMonth.MonthValue expectedMonth)
            {
                var yearMonth = new YearMonth {MonthCount = _sut,};

                yearMonth.Month.ShouldBe(expectedMonth);
            }
        }

        public class VerifyYearSetter
        {
            private int _sutYear;
            private YearMonth.MonthValue _sutMonth;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("year", "month", "expectedIsExceptionThrown")
                    {
                        {0999, YearMonth.MonthValue.January, true},
                        {1000, YearMonth.MonthValue.January, false},
                        {9999, YearMonth.MonthValue.January, false},
                        {-001, YearMonth.MonthValue.January, true},
                    })
                    .BDDfy();
            }

            public void GivenAMonth(YearMonth.MonthValue month)
            {
                _sutMonth = month;
            }

            public void AndGivenAYear(int year)
            {
                _sutYear = year;
            }

            public void ThenAnExceptionIsThrownAsExpected(bool expectedIsExceptionThrown)
            {
                if (expectedIsExceptionThrown)
                    Assert.Throws<System.ArgumentOutOfRangeException>(
                        () => new YearMonth {Year = _sutYear, Month = _sutMonth});
            }
        }

        public class VerifyYearAndMonth
        {
            private YearMonth _sut;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("SUT", "Expected Year", "Expected Month")
                    {
                        {
                            new YearMonth {Year = 2016, Month = YearMonth.MonthValue.January}, 2016,
                            YearMonth.MonthValue.January
                        },
                        {
                            new YearMonth {Year = 9999, Month = YearMonth.MonthValue.December}, 9999,
                            YearMonth.MonthValue.December
                        },
                        {
                            new YearMonth {Year = 1000, Month = YearMonth.MonthValue.January}, 1000,
                            YearMonth.MonthValue.January
                        },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(YearMonth sut)
            {
                _sut = sut;
            }

            public void ThenYearShouldBe(int expectedYear)
            {
                _sut.Year.ShouldBe(expectedYear);
            }

            public void AndMonthShouldBe(YearMonth.MonthValue expectedMonth)
            {
                _sut.Month.ShouldBe(expectedMonth);
            }
        }

        public class VerifyDaysInMonth
        {
            private YearMonth _sut;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("Sut", "Expected DaysInMonth")
                    {
                        {
                            new YearMonth
                            {
                                Year = YearMonth.YearLimit.Lower,
                                Month = YearMonth.MonthValue.January
                            },
                            31
                        },
                        { new YearMonth {Year = 2000, Month = YearMonth.MonthValue.February}, 29},
                        { new YearMonth {Year = 2001, Month = YearMonth.MonthValue.February}, 28},
                        { new YearMonth {Year = 2001, Month = YearMonth.MonthValue.March}, 31},
                        { new YearMonth {Year = 2001, Month = YearMonth.MonthValue.April}, 30},
                        {
                            new Scheduler.YearMonth
                            {
                                Year = YearMonth.YearLimit.Upper,
                                Month = YearMonth.MonthValue.December
                            },
                            31
                        },
                    })
                    .BDDfy();
            }

            public void GivenARepeatingDay(Scheduler.YearMonth sut)
            {
                _sut = sut;
            }

            public void ThenYearShouldBe(int expectedDaysInMonth)
            {
                _sut.DaysInMonth.ShouldBe(expectedDaysInMonth);
            }
        }

        public class VerifyRangeInvalid
        {
            YearMonth _sutFrom;
            YearMonth _sutTo;


            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("yearMonthFrom", "yearMonthTo")
                    {
                        {
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.January},
                            new YearMonth {Year = 1999, Month = YearMonth.MonthValue.April}
                        },
                        {
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.February},
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.January}
                        },
                    })
                    .BDDfy();
            }

            public void GivenFromAndToYearMonths(YearMonth yearMonthFrom, YearMonth yearMonthTo)
            {
                _sutFrom = yearMonthFrom;
                _sutTo = yearMonthTo;

            }

            public void ThenAnExceptionShouldBeThrown()
            {
                Assert.Throws<System.ArgumentOutOfRangeException>(() => YearMonth.Range(_sutFrom, _sutTo));
            }
        }

        public class VerifyRange
        {
            YearMonth _sutFrom;
            YearMonth _sutTo;

            [Fact]
            public void Execute()
            {
                this.WithExamples(new ExampleTable("yearMonthFrom", "yearMonthTo", "expectedMonths")
                    {
                        {
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.January},
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.January}, 1
                        },
                        {
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.January},
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.April}, 4
                        },
                        {
                            new YearMonth {Year = 2000, Month = YearMonth.MonthValue.February},
                            new YearMonth {Year = 2001, Month = YearMonth.MonthValue.January}, 12
                        },
                    })
                    .BDDfy();
            }

            public void GivenFromAndToYearMonths(YearMonth yearMonthFrom, YearMonth yearMonthTo)
            {
                _sutFrom = yearMonthFrom;
                _sutTo = yearMonthTo;

            }

            public void ThenTheCorrectNumberOfMonthsAreCreated(int expectedMonths)
            {
                var yearMonths = YearMonth.Range(_sutFrom, _sutTo);

                yearMonths.Count().ShouldBe(expectedMonths);
            }
        }
    }
}