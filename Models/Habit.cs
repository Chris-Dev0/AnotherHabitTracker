using System;

namespace AnotherHabitTracker.Models;

public class Habit
{
    public Guid HabitId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly CreatedDate { get; set; }
}
