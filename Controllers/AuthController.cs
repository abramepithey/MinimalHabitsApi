using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalHabitsApi.Data;
using MinimalHabitsApi.DTOs;
using MinimalHabitsApi.Models;
using MinimalHabitsApi.Services;
using System.Security.Claims;

namespace MinimalHabitsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public AuthController(ApplicationDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [HttpPost("register")]
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

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username, cancellationToken);
        if (user == null)
            return Unauthorized("Invalid username");

        if (!_authService.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid password");

        var token = await _authService.CreateTokenAsync(user, cancellationToken);
        return Ok(token);
    }
} 