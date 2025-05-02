using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Data;
using MinimalApi.DTOs;
using MinimalApi.Models;
using MinimalApi.Services;

namespace MinimalApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly HabitService _habitService;

    public HabitsController(HabitService habitService)
    {
        _habitService = habitService;
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
        var habit = await _habitService.CreateHabitAsync(userId, habitDto);
        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit);
    }

    [HttpGet]
    public async Task<ActionResult<List<Habit>>> GetHabits()
    {
        var userId = GetCurrentUserId();
        return await _habitService.GetUserHabitsAsync(userId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Habit>> GetHabit(int id)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.GetHabitAsync(userId, id);
        if (habit == null)
            return NotFound();
        return habit;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHabit(int id, HabitDto habitDto)
    {
        var userId = GetCurrentUserId();
        var success = await _habitService.UpdateHabitAsync(userId, id, habitDto);
        if (!success)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHabit(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _habitService.DeleteHabitAsync(userId, id);
        if (!success)
            return NotFound();
        return NoContent();
    }

    [HttpPost("{habitId}/entries")]
    public async Task<ActionResult<HabitEntry>> AddHabitEntry(int habitId, HabitEntryDto entryDto)
    {
        var userId = GetCurrentUserId();
        var entry = await _habitService.AddHabitEntryAsync(userId, habitId, entryDto);
        if (entry == null)
            return NotFound("Habit not found");
        return CreatedAtAction(nameof(GetHabit), new { id = habitId }, entry);
    }

    [HttpGet("{habitId}/entries")]
    public async Task<ActionResult<List<HabitEntry>>> GetHabitEntries(int habitId)
    {
        var userId = GetCurrentUserId();
        var entries = await _habitService.GetHabitEntriesAsync(userId, habitId);
        if (entries.Count == 0)
            return NotFound("Habit not found");
        return entries;
    }
} 