using System;

namespace AnotherHabitTracker.Models;

public class Completion
{
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
}
