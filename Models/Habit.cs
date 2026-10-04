using System;
using System.Collections.Generic;

namespace AnotherHabitTracker.Models;


public class Habit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<HabitRecord> Records = new();
}