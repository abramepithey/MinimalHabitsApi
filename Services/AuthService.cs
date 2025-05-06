using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MinimalHabitsApi.Models;

namespace MinimalHabitsApi.Services;

/// <summary>
/// Service for handling authentication-related operations.
/// </summary>
public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration is missing or invalid.</exception>
    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured");
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        _key = new SymmetricSecurityKey(keyBytes);
    }

    /// <summary>
    /// Creates a password hash and salt for a given password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="passwordHash">The resulting password hash.</param>
    /// <param name="passwordSalt">The salt used for hashing.</param>
    public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    /// <summary>
    /// Verifies if a password matches its hash and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The stored password hash.</param>
    /// <param name="passwordSalt">The stored password salt.</param>
    /// <returns>True if the password matches, false otherwise.</returns>
    public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    /// <summary>
    /// Creates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to create the token for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A JWT token string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration is missing or invalid.</exception>
    public Task<string> CreateTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = creds,
            Issuer = issuer,
            Audience = audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
} 