namespace MinimalHabitsApi.DTOs;

/// <summary>
/// Data transfer object for habit creation and updates.
/// </summary>
public class HabitDto
{
    /// <summary>
    /// The name of the habit.
    /// </summary>
    public string Name { get; set; } = string.Empty;
} 