namespace MinimalApi.Models;

public class Habit
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<HabitEntry> Entries { get; set; } = new();
} 