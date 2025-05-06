namespace MinimalHabitsApi.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The hashed password of the user.
    /// </summary>
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The salt used for password hashing.
    /// </summary>
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The collection of habits associated with this user.
    /// </summary>
    public List<Habit> Habits { get; set; } = new();
} 