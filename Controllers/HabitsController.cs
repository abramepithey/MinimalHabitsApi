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
/// Controller for managing user habits and habit entries.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HabitsController : ControllerBase
{
    private readonly HabitService _habitService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HabitsController"/> class.
    /// </summary>
    /// <param name="habitService">The habit service.</param>
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

    /// <summary>
    /// Creates a new habit for the current user.
    /// </summary>
    /// <param name="habitDto">The habit data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created habit.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Habit), 201)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<Habit>> CreateHabit(HabitDto habitDto, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.CreateHabitAsync(userId, habitDto, cancellationToken);
        return CreatedAtAction(nameof(GetHabit), new { id = habit.Id }, habit);
    }

    /// <summary>
    /// Gets all habits for the current user.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of habits.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<Habit>), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<Habit>>> GetHabits(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        return await _habitService.GetUserHabitsAsync(userId, cancellationToken);
    }

    /// <summary>
    /// Gets a specific habit by ID for the current user.
    /// </summary>
    /// <param name="id">The habit ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The habit if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Habit), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<Habit>> GetHabit(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var habit = await _habitService.GetHabitAsync(userId, id, cancellationToken);
        if (habit == null)
            return NotFound();
        return habit;
    }

    /// <summary>
    /// Updates a specific habit for the current user.
    /// </summary>
    /// <param name="id">The habit ID.</param>
    /// <param name="habitDto">The updated habit data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content if successful.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> UpdateHabit(int id, HabitDto habitDto, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var success = await _habitService.UpdateHabitAsync(userId, id, habitDto, cancellationToken);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Deletes a specific habit for the current user.
    /// </summary>
    /// <param name="id">The habit ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> DeleteHabit(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var success = await _habitService.DeleteHabitAsync(userId, id, cancellationToken);
        if (!success)
            return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Adds a new entry to a specific habit for the current user.
    /// </summary>
    /// <param name="habitId">The habit ID.</param>
    /// <param name="entryDto">The entry data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created entry.</returns>
    [HttpPost("{habitId}/entries")]
    [ProducesResponseType(typeof(HabitEntry), 201)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<HabitEntry>> AddHabitEntry(int habitId, HabitEntryDto entryDto, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var entry = await _habitService.AddHabitEntryAsync(userId, habitId, entryDto, cancellationToken);
        if (entry == null)
            return NotFound("Habit not found");
        return CreatedAtAction(nameof(GetHabit), new { id = habitId }, entry);
    }

    /// <summary>
    /// Gets all entries for a specific habit for the current user.
    /// </summary>
    /// <param name="habitId">The habit ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of entries.</returns>
    [HttpGet("{habitId}/entries")]
    [ProducesResponseType(typeof(List<HabitEntry>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<List<HabitEntry>>> GetHabitEntries(int habitId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var entries = await _habitService.GetHabitEntriesAsync(userId, habitId, cancellationToken);
        if (entries.Count == 0)
            return NotFound("Habit not found");
        return entries;
    }
} 