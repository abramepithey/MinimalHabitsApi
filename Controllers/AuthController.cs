using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalHabitsApi.Data;
using MinimalHabitsApi.DTOs;
using MinimalHabitsApi.Models;
using MinimalHabitsApi.Services;
using System.Security.Claims;

namespace MinimalHabitsApi.Controllers;

/// <summary>
/// Controller for handling user authentication operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="authService">The authentication service.</param>
    public AuthController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDto">The registration data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created user.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<User>> Register(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username, cancellationToken))
            return BadRequest("Username is already taken");

        _authService.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(user);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A JWT token if successful.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<object>> Login(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username, cancellationToken);
        if (user == null)
            return Unauthorized("Invalid username");

        if (!_authService.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid password");

        var token = await _authService.CreateTokenAsync(user, cancellationToken);
        return Ok(new { token });
    }
} 