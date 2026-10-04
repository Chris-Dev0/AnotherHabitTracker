using System;
using System.Collections.Generic;

namespace AnotherHabitTracker.Models;


public class HabitRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }

    public bool IsCompleted { get; set; }

}