using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using AnotherHabitTracker.Models;

namespace AnotherHabitTracker.Services;

public class JsonStorageService
{
    public string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AnotherHabitTracker", "habits.json");

    public HabitData Load()
    {
        if (!File.Exists(FilePath))
            return new HabitData();

        var data = JsonSerializer.Deserialize<HabitData>(File.ReadAllText(FilePath))
                   ?? throw new InvalidDataException("The JSON file is empty or invalid.");

        if (data.Habits is null || data.Completions is null ||
            data.Habits.Any(h => h is null || h.HabitId == Guid.Empty ||
                string.IsNullOrWhiteSpace(h.Name) || h.Name.Trim().Length > 60) ||
            data.Habits.Select(h => h.HabitId).Distinct().Count() != data.Habits.Count ||
            data.Completions.Any(c => c is null || !data.Habits.Any(h =>
                h.HabitId == c.HabitId && c.Date >= h.CreatedDate)) ||
            data.Completions.Select(c => (c.HabitId, c.Date)).Distinct().Count() != data.Completions.Count)
        {
            throw new InvalidDataException("Saved habit data failed validation.");
        }

        return data;
    }

    public void Save(HabitData data)
    {
        string directory = Path.GetDirectoryName(FilePath)!;
        Directory.CreateDirectory(directory);

        // Write a separate temporary file before replacing the old one.
        string temporaryPath = FilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(temporaryPath, json);
            File.Move(temporaryPath, FilePath, overwrite: true);
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            }
            catch (IOException) { /* Cleanup is best effort. */ }
            catch (UnauthorizedAccessException) { /* Cleanup is best effort. */ }
        }
    }
}
