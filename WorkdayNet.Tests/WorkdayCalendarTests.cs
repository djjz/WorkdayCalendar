namespace WorkdayNet.Tests;

public class WorkdayCalendarTests
{
    public static TheoryData<DateTime, decimal, DateTime> ImplementationSheetExamples = new()
    {
        { new DateTime(2004, 05, 24, 18, 05, 00), -5.5m, new DateTime(2004, 05, 14, 12, 00, 00) },
        { new DateTime(2004, 05, 24, 19, 03, 00), 44.723656m, new DateTime(2004, 07, 27, 13, 47, 00) },
        { new DateTime(2004, 05, 24, 18, 03, 00), -6.7470217m, new DateTime(2004, 05, 13, 10, 02, 00) },
        { new DateTime(2004, 05, 24, 08, 03, 00), 12.782709m, new DateTime(2004, 06, 10, 14, 18, 00) },
        { new DateTime(2004, 05, 24, 07, 03, 00), 8.276628m, new DateTime(2004, 06, 04, 10, 12, 00) },
    };

    [Theory]
    [MemberData(nameof(ImplementationSheetExamples))]
    public void When_UsingImplementationSheetExamples_Then_CalendarReturnsExpectedResult(
        DateTime startDate,
        decimal increment,
        DateTime expectedDate
    )
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        workdayCalendar.SetRecurringHoliday(5, 17);
        workdayCalendar.SetHoliday(new DateTime(2004, 5, 27));

        var incrementedDate = workdayCalendar.GetWorkdayIncrement(startDate, increment);

        Assert.Equal(
            expected: expectedDate,
            actual: incrementedDate
        );
    }
}