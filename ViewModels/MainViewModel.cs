using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using AnotherHabitTracker.Models;
using AnotherHabitTracker.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnotherHabitTracker.ViewModels;

public class HabitRow
{
    public Guid HabitId { get; }
    public string Name { get; }
    public string TodayStatus { get; }

    public HabitRow(Guid habitId, string name, string todayStatus)
    {
        HabitId = habitId;
        Name = name;
        TodayStatus = todayStatus;
    }
}

public partial class MainViewModel : ViewModelBase
{
    private readonly JsonStorageService _storage = new();
    private HabitData _data = new();
    private bool _canSave = true;

    [ObservableProperty]
    private string newHabitName = "";

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private HabitRow? selectedHabit;

    public ObservableCollection<HabitRow> HabitList { get; } = new();
    public string TodayLabel => $"Today: {DateTime.Today:yyyy-MM-dd}";

    public MainViewModel()
    {
        try
        {
            _data = _storage.Load();
            RefreshHabitList();
        }
        catch (Exception ex)
        {
            // Do not overwrite an existing JSON file if it could not be loaded.
            _canSave = false;
            ErrorMessage = $"Unable to load saved habits: {ex.Message}. The file was not changed.";
        }
    }

    [RelayCommand]
    private void AddHabit()
    {
        if (!CanSave()) return;

        string name = (NewHabitName ?? "").Trim();
        if (name.Length is < 1 or > 60)
        {
            ErrorMessage = "Habit name must be 1 to 60 characters.";
            return;
        }

        var candidate = CopyData();
        var habit = new Habit
        {
            HabitId = Guid.NewGuid(),
            Name = name,
            CreatedDate = DateOnly.FromDateTime(DateTime.Today)
        };
        candidate.Habits.Add(habit);

        if (SaveCandidate(candidate, habit.HabitId)) NewHabitName = "";
    }

    [RelayCommand]
    private void DeleteHabit()
    {
        if (!CanSave()) return;
        if (SelectedHabit is null)
        {
            ErrorMessage = "Please select a habit first.";
            return;
        }

        Guid habitId = SelectedHabit.HabitId;
        var candidate = CopyData();
        candidate.Habits.RemoveAll(h => h.HabitId == habitId);
        candidate.Completions.RemoveAll(c => c.HabitId == habitId);
        SaveCandidate(candidate);
    }

    [RelayCommand]
    private void ToggleCompletion()
    {
        if (!CanSave()) return;
        if (SelectedHabit is null)
        {
            ErrorMessage = "Please select a habit first.";
            return;
        }

        Guid habitId = SelectedHabit.HabitId;
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        Habit? habit = _data.Habits.FirstOrDefault(h => h.HabitId == habitId);
        if (habit is null || today < habit.CreatedDate)
        {
            ErrorMessage = "This habit cannot be completed for today's date.";
            return;
        }

        var candidate = CopyData();
        Completion? existing = candidate.Completions.FirstOrDefault(c =>
            c.HabitId == habitId && c.Date == today);
        if (existing is null)
            candidate.Completions.Add(new Completion { HabitId = habitId, Date = today });
        else
            candidate.Completions.Remove(existing);

        SaveCandidate(candidate, habitId);
    }

    private HabitData CopyData() => new()
    {
        Habits = new List<Habit>(_data.Habits),
        Completions = new List<Completion>(_data.Completions)
    };

    private bool CanSave()
    {
        if (_canSave) return true;
        ErrorMessage = "Saved data could not be loaded. Close the app and check the JSON file before making changes.";
        return false;
    }

    private bool SaveCandidate(HabitData candidate, Guid? selectHabitId = null)
    {
        try
        {
            _storage.Save(candidate);
            _data = candidate;
            RefreshHabitList(selectHabitId);
            ErrorMessage = "";
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not save changes: {ex.Message}";
            return false;
        }
    }

    private void RefreshHabitList(Guid? selectHabitId = null)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        HabitList.Clear();
        foreach (Habit habit in _data.Habits)
        {
            string status = today < habit.CreatedDate ? "Not applicable" :
                _data.Completions.Any(c => c.HabitId == habit.HabitId && c.Date == today)
                    ? "Completed" : "Not completed";
            HabitList.Add(new HabitRow(habit.HabitId, habit.Name, status));
        }
        SelectedHabit = selectHabitId is Guid id
            ? HabitList.FirstOrDefault(h => h.HabitId == id)
            : null;
        OnPropertyChanged(nameof(TodayLabel));
    }
}
