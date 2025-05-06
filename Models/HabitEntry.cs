namespace MinimalHabitsApi.Models;

/// <summary>
/// Represents a single entry tracking the completion of a habit on a specific date.
/// </summary>
public class HabitEntry
{
    /// <summary>
    /// The unique identifier for the habit entry.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The date this entry represents.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Whether the habit was completed on this date.
    /// </summary>
    public bool Completed { get; set; }

    /// <summary>
    /// The ID of the habit this entry belongs to.
    /// </summary>
    public int HabitId { get; set; }

    /// <summary>
    /// The habit this entry belongs to.
    /// </summary>
    public Habit Habit { get; set; } = null!;
} 