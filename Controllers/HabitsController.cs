using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.DTOs;
using MinimalApi.Models;

namespace MinimalApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HabitsController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in token");
        return int.Parse(userIdClaim.Value);
    }

    [HttpPost]
    public async Task<ActionResult<Habit>> CreateHabit(HabitDto habitDto)
    {
        var userId = GetCurrentUserId();
        var habit = new Habit
        {
            Name = habitDto.Name,
            UserId = userId
        };

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit);
    }
} 