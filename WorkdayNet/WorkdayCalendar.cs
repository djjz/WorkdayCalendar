namespace WorkdayNet;

public class WorkdayCalendar : IWorkdayCalendar
{
    private readonly HashSet<RecurringHoliday> RecurringHolidays = [];
    private readonly HashSet<DateOnly> Holidays = [];
    private TimeOnly? WorkdayStart;
    private TimeOnly? WorkdayStop;

    private readonly int[] DaysInMonth = [ 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 ];

    public void SetHoliday(DateTime date) 
        => Holidays.Add(DateOnly.FromDateTime(date));

    public void SetRecurringHoliday(int month, int day)
    {
        ValidateDayOfMonth(month, day);
        RecurringHolidays.Add(new RecurringHoliday(month, day));
    }

    private void ValidateDayOfMonth(int month, int day)
    {
        if (month < 1 || month > 12)
            throw new ArgumentException("Month has to be between 1 and 12");

        if (day < 1)
            throw new ArgumentException("Day has to be greater than 0");

        if (day > DaysInMonth[month - 1])
            throw new AggregateException("Day is outside of the possible days in the month");
    }

    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        ValidateHour(startHours, startMinutes);
        ValidateHour(stopHours, stopMinutes);

        WorkdayStart = new TimeOnly(startHours, startMinutes);
        WorkdayStop = new TimeOnly(stopHours, stopMinutes);
    }

    private void ValidateHour(int hour, int minute)
    {
        if (hour < 0 || hour > 23)
            throw new ArgumentException("Hour has to be between 0 and 23");

        if (minute < 0 || minute > 59)
            throw new ArgumentException("Minute has to be between 0 and 59");
    }

    private void ValidateWorkday()
    {
        if (WorkdayStart is null ||  WorkdayStop is null)
            throw new InvalidOperationException("Workday start and stop are required to calculate increments");
    }

    private bool IsHoliday(DateOnly date) =>
        date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
        || Holidays.Contains(date)
        || RecurringHolidays.Any(a => a.Month == date.Month && a.Day == date.Day);

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        ValidateWorkday();
        var scrollDirection = Math.Sign(incrementInWorkdays);
        var incrementsRemaining = Math.Abs(incrementInWorkdays);

        // If starting date is outside work hours, snap to next day in scroll direction
        var startingTimeOfDay = TimeOnly.FromDateTime(startDate);
        var currentDate = DateOnly.FromDateTime(startDate);
        
        /* If starting time is past (before when scrolling backwards, after if forwards)
         * work hours, add one more increment, to let the loop handle the additional date bump */
        if ((scrollDirection > 0 && startingTimeOfDay > WorkdayStop) 
            || (scrollDirection < 0 && startingTimeOfDay < WorkdayStart))
            incrementsRemaining++;
         
        // Scroll days
        while (incrementsRemaining > 1 || IsHoliday(currentDate))
        {
            if (IsHoliday(currentDate))
            {
                currentDate = currentDate.AddDays(scrollDirection);
                continue;
            }

            currentDate = currentDate.AddDays(scrollDirection);
            incrementsRemaining -= 1;
        }
        
        // Calculate time of day
        // Get starting time at the beginning or the end of the workday
        var startTime = startingTimeOfDay.IsBetween(WorkdayStart!.Value, WorkdayStop!.Value)
                ? startingTimeOfDay // Use original time if inside business hours
                : scrollDirection > 0 // Otherwise use workday start/stop
                    ? WorkdayStart.Value
                    : WorkdayStop.Value;
                
        // Calculate time to add
        var timeToAdd = Math.Truncate(
            decimal.CreateChecked((WorkdayStop - WorkdayStart).Value.TotalMinutes) * incrementsRemaining
        );
        return currentDate.ToDateTime(startTime.AddMinutes(double.CreateChecked(timeToAdd) * scrollDirection));
    } 
}
