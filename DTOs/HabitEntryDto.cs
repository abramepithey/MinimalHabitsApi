namespace MinimalHabitsApi.DTOs;

/// <summary>
/// Data transfer object for habit entry creation and updates.
/// </summary>
public class HabitEntryDto
{
    /// <summary>
    /// The date of the habit entry.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Whether the habit was completed on this date.
    /// </summary>
    public bool Completed { get; set; }
} 