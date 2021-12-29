using System.Collections.Generic;
using QuartzCronGenerator;
using Xunit;

namespace QuartzCronGeneratorLibrary.Tests;

public class Tests
{
    [Theory]
    [InlineData(1)]
    [InlineData(59)]
    [InlineData(3600)]
    public void TestEveryNSeconds(int seconds)
    {
        var actual = CronExpression.EveryNSeconds(seconds);
        Assert.Equal($"0/{seconds} * * 1/1 * ? *", actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(59)]
    [InlineData(3600)]
    public void TestEveryNMinutes(int minutes)
    {
        var actual = CronExpression.EveryNMinutes(minutes);
        Assert.Equal($"0 0/{minutes} * 1/1 * ? *", actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(23)]
    [InlineData(72)]
    public void TestEveryNHours(int hours)
    {
        var actual = CronExpression.EveryNHours(hours);
        Assert.Equal($"0 0 0/{hours} 1/1 * ? *", actual);
    }

    [Theory]
    [InlineData(12, 0)]
    [InlineData(7, 23)]
    [InlineData(22, 22)]
    public void TestEveryDayAt(int hour, int minute)
    {
        var ce1 = CronExpression.EveryDayAt(hour, minute);
        Assert.Equal($"0 {minute} {hour} 1/1 * ? *", ce1);
    }

    [Theory]
    [InlineData(1, 12, 0)]
    [InlineData(6, 7, 23)]
    [InlineData(30, 22, 22)]
    public void TestEveryNDaysAt(int days, int hour, int minute)
    {
        var ce1 = CronExpression.EveryNDaysAt(days, hour, minute);
        Assert.Equal($"0 {minute} {hour} 1/{days} * ? *", ce1);
    }

    [Theory]
    [InlineData(12, 0)]
    [InlineData(7, 23)]
    [InlineData(22, 22)]
    public void TestEveryWeekDay(int hour, int minute)
    {
        var actual = CronExpression.EveryWeekDayAt(hour, minute);
        Assert.Equal($"0 {minute} {hour} ? * MON-FRI *", actual);
    }

    [Theory]
    [InlineData(12, 0, DaysOfWeek.Monday)]
    [InlineData(7, 23, DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday)]
    [InlineData(22, 22, DaysOfWeek.Saturday | DaysOfWeek.Sunday)]
    public void TestEverySpecificWeekDayAt(int hour, int minute, DaysOfWeek daysOfWeek)
    {
        var actual = CronExpression.EverySpecificWeekDayAt(hour, minute, daysOfWeek);
        Assert.Equal($"0 {minute} {hour} ? * {CronConverter.ToCronRepresentation(daysOfWeek)} *", actual);
    }

    [Theory]
    [InlineData(DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday, "MON,WED,FRI")]
    [InlineData(DaysOfWeek.Saturday | DaysOfWeek.Sunday, "SAT,SUN")]
    [InlineData(DaysOfWeek.Sunday | DaysOfWeek.Saturday, "SAT,SUN")]
    public void TestDaysOfWeekRepresentation(DaysOfWeek daysOfWeek, string expectedRepresentation)
    {
        Assert.Equal(expectedRepresentation, CronConverter.ToCronRepresentation(daysOfWeek));
    }

    [Fact]
    private void TestCronConverterGetFlags()
    {
        const DaysOfWeek days = DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday;
        var daysList = new List<DaysOfWeek> {DaysOfWeek.Monday, DaysOfWeek.Wednesday, DaysOfWeek.Friday};
        Assert.Equal(daysList, CronConverter.GetFlags(days));
    }

    [Theory]
    [InlineData(DaysOfWeek.Monday, "MON")]
    [InlineData(DaysOfWeek.Tuesday, "TUE")]
    [InlineData(DaysOfWeek.Wednesday, "WED")]
    [InlineData(DaysOfWeek.Thursday, "THU")]
    [InlineData(DaysOfWeek.Friday, "FRI")]
    [InlineData(DaysOfWeek.Saturday, "SAT")]
    [InlineData(DaysOfWeek.Sunday, "SUN")]
    public void TestDaysOfWeekRepresentationSingle(DaysOfWeek dayOfWeek, string expectedRepresentation)
    {
        var actual = CronConverter.ToCronRepresentationSingle(dayOfWeek);
        Assert.Equal(expectedRepresentation, actual);
    }

    [Theory]
    [InlineData(1, 1, 12, 0)]
    [InlineData(7, 3, 7, 15)]
    [InlineData(28, 6, 21, 30)]
    public void TestEverySpecificDayEveryNMonthAt(int dayNumber, int monthInterval, int hour, int minute)
    {
        var actual = CronExpression.EverySpecificDayEveryNMonthAt(dayNumber, monthInterval, hour, minute);
        Assert.Equal($"0 {minute} {hour} {dayNumber} 1/{monthInterval} ? *", actual);
    }

    [Theory]
    [InlineData(DaySeqNumber.First, DaysOfWeek.Monday, 1, 12, 0)]
    [InlineData(DaySeqNumber.Second, DaysOfWeek.Wednesday, 3, 7, 15)]
    [InlineData(DaySeqNumber.Third, DaysOfWeek.Friday, 6, 21, 30)]
    [InlineData(DaySeqNumber.Fourth, DaysOfWeek.Sunday, 77, 22, 45)]
    public void TestEverySpecificSeqWeekDayEveryNMonthAt(DaySeqNumber daySeqNumber, DaysOfWeek dayOfWeek,
        int monthInterval, int hour, int minute)
    {
        var actual =
            CronExpression.EverySpecificSeqWeekDayEveryNMonthAt(daySeqNumber, dayOfWeek, monthInterval, hour, minute);
        Assert.Equal(
            $"0 {minute} {hour} ? 1/{monthInterval} {CronConverter.ToCronRepresentationSingle(dayOfWeek)}#{(int) daySeqNumber} *",
            actual);
    }

    [Theory]
    [InlineData(Months.January, DaysOfWeek.Monday, 12, 0)]
    [InlineData(Months.February, DaysOfWeek.Wednesday, 15, 17)]
    [InlineData(Months.August, DaysOfWeek.Friday, 20, 45)]
    [InlineData(Months.December, DaysOfWeek.Sunday, 23, 59)]
    public void TestEverySpecificDayOfMonthAt(Months month, int dayNumber, int hour, int minute)
    {
        var actual = CronExpression.EverySpecificDayOfMonthAt(month, dayNumber, hour, minute);
        Assert.Equal($"0 {minute} {hour} {dayNumber} {(int) month} ? *", actual);
    }

    [Theory]
    [InlineData(DaySeqNumber.First, DaysOfWeek.Monday, Months.January, 12, 0)]
    [InlineData(DaySeqNumber.Second, DaysOfWeek.Wednesday, Months.February, 7, 15)]
    [InlineData(DaySeqNumber.Third, DaysOfWeek.Friday, Months.August, 21, 30)]
    [InlineData(DaySeqNumber.Fourth, DaysOfWeek.Sunday, Months.December, 22, 45)]
    public void TestEverySpecificSeqWeekDayOfMonthAt(DaySeqNumber daySeqNumber, DaysOfWeek dayOfWeek, Months month,
        int hour, int minute)
    {
        var ce1 = CronExpression.EverySpecificSeqWeekDayOfMonthAt(daySeqNumber, dayOfWeek, month,
            hour, minute);
        Assert.Equal(
            $"0 {minute} {hour} ? {(int) month} {CronConverter.ToCronRepresentationSingle(dayOfWeek)}#{(int) daySeqNumber} *",
            ce1);
    }
}