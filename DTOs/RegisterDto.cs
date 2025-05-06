namespace MinimalHabitsApi.DTOs;

/// <summary>
/// Data transfer object for user registration requests.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// The desired username for the new user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password for the new user account.
    /// </summary>
    public string Password { get; set; } = string.Empty;
} 