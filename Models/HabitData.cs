using System.Collections.Generic;

namespace AnotherHabitTracker.Models;

// JSON container. Its "Habits" property holds individual Habit objects.
public class HabitData
{
    public List<Habit> Habits { get; set; } = new();
    public List<Completion> Completions { get; set; } = new();
}
