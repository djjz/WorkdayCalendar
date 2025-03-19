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
    
    [Fact]
    public void When_AddingLessThanFullWorkday_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 08, 00, 00),
            0.25m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 19, 10, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingLessThanFullWorkday_AndStartDateIsBeforeBusinessHours_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 03, 00, 00),
            0.25m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 19, 10, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingLessThanFullWorkday_AndStartDateIsAfterBusinessHours_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 20, 00, 00),
            0.25m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 20, 10, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingFullDays_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 08, 00, 00),
            4m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 25, 08, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingFullDaysAndStartDateIsBeforeBusinessHours_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 00, 00, 00),
            4m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 25, 08, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingFullDaysAndStartDateIsAfterBusinessHours_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 19, 00, 00),
            4m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 26, 08, 00, 00),
            actual: incrementedDate
        );
    }

    [Fact]
    public void When_CalculatingLongWorkingHours_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 20, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 19, 2, 00, 00),
            1.5m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 20, 14, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingFullDaysAndBeginningOnSaturday_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 22, 02, 17, 00),
            0.75m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 24, 14, 00, 00),
            actual: incrementedDate
        );
    }
    
    [Fact]
    public void When_AddingFullDaysAndBeginningOnSunday_CalendarWorks()
    {
        IWorkdayCalendar workdayCalendar = new WorkdayCalendar();
        workdayCalendar.SetWorkdayStartAndStop(8, 0, 16, 0);
    
        var incrementedDate = workdayCalendar.GetWorkdayIncrement(
            new DateTime(2025, 03, 23, 08, 00, 00),
            0.75m
        );
    
        Assert.Equal(
            expected: new DateTime(2025, 03, 24, 14, 00, 00),
            actual: incrementedDate
        );
    }
}
