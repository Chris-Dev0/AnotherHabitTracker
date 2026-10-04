

using System;
using System.Collections.Generic;
using AnotherHabitTracker.Models;

namespace AnotherHabitTracker.Services;


public interface IHabitService
{
    void ToggleCompletion(Guid id, DateOnly date);
}
public class HabitService() : IHabitService
{
    public List<Habit> Habits { get; private set; } = new();

    public void ToggleCompletion(Guid id, DateOnly date)
    {

    }
}