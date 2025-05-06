namespace MinimalHabitsApi.DTOs;

/// <summary>
/// Data transfer object for user login requests.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// The username of the user attempting to log in.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password of the user attempting to log in.
    /// </summary>
    public string Password { get; set; } = string.Empty;
} 