namespace MinimalHabitsApi.Models;

/// <summary>
/// Represents a habit that a user wants to track.
/// </summary>
public class Habit
{
    /// <summary>
    /// The unique identifier for the habit.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the habit.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the user who owns this habit.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The user who owns this habit.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The collection of entries tracking this habit's completion.
    /// </summary>
    public List<HabitEntry> Entries { get; set; } = new();
} 