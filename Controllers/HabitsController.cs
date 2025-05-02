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

    [HttpGet]
    public async Task<ActionResult<List<Habit>>> GetHabits()
    {
        var userId = GetCurrentUserId();
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Entries)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Habit>> GetHabit(int id)
    {
        var userId = GetCurrentUserId();
        var habit = await _context.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

        if (habit == null)
            return NotFound();

        return habit;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHabit(int id, HabitDto habitDto)
    {
        var userId = GetCurrentUserId();
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

        if (habit == null)
            return NotFound();

        habit.Name = habitDto.Name;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHabit(int id)
    {
        var userId = GetCurrentUserId();
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

        if (habit == null)
            return NotFound();

        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{habitId}/entries")]
    public async Task<ActionResult<HabitEntry>> AddHabitEntry(int habitId, HabitEntryDto entryDto)
    {
        var userId = GetCurrentUserId();
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

        if (habit == null)
            return NotFound("Habit not found");

        var entry = new HabitEntry
        {
            Date = entryDto.Date,
            Completed = entryDto.Completed,
            HabitId = habitId
        };

        _context.HabitEntries.Add(entry);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetHabit), new { id = habitId }, entry);
    }
} 