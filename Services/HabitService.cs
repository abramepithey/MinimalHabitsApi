using Microsoft.EntityFrameworkCore;
using MinimalHabitsApi.Data;
using MinimalHabitsApi.DTOs;
using MinimalHabitsApi.Models;

namespace MinimalHabitsApi.Services;

/// <summary>
/// Service for managing habits and habit entries.
/// </summary>
public class HabitService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="HabitService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public HabitService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new habit for a user.
    /// </summary>
    /// <param name="userId">The ID of the user creating the habit.</param>
    /// <param name="habitDto">The habit data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created habit.</returns>
    public async Task<Habit> CreateHabitAsync(int userId, HabitDto habitDto, CancellationToken cancellationToken = default)
    {
        var habit = new Habit
        {
            Name = habitDto.Name,
            UserId = userId
        };

        await _context.Habits.AddAsync(habit, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return habit;
    }

    /// <summary>
    /// Gets all habits for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of the user's habits.</returns>
    public async Task<List<Habit>> GetUserHabitsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Entries)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a specific habit by ID for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="habitId">The ID of the habit.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The habit if found, null otherwise.</returns>
    public async Task<Habit?> GetHabitAsync(int userId, int habitId, CancellationToken cancellationToken = default)
    {
        return await _context.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// Updates a specific habit for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="habitId">The ID of the habit to update.</param>
    /// <param name="habitDto">The updated habit data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the habit was updated, false if not found.</returns>
    public async Task<bool> UpdateHabitAsync(int userId, int habitId, HabitDto habitDto, CancellationToken cancellationToken = default)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);

        if (habit == null)
            return false;

        habit.Name = habitDto.Name;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Deletes a specific habit for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="habitId">The ID of the habit to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if the habit was deleted, false if not found.</returns>
    public async Task<bool> DeleteHabitAsync(int userId, int habitId, CancellationToken cancellationToken = default)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);

        if (habit == null)
            return false;

        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Adds a new entry to a specific habit for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="habitId">The ID of the habit.</param>
    /// <param name="entryDto">The entry data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created entry if successful, null if the habit was not found.</returns>
    public async Task<HabitEntry?> AddHabitEntryAsync(int userId, int habitId, HabitEntryDto entryDto, CancellationToken cancellationToken = default)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);

        if (habit == null)
            return null;

        var entry = new HabitEntry
        {
            Date = entryDto.Date,
            Completed = entryDto.Completed,
            HabitId = habitId
        };

        await _context.HabitEntries.AddAsync(entry, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    /// <summary>
    /// Gets all entries for a specific habit for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="habitId">The ID of the habit.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of entries for the habit, empty list if the habit was not found.</returns>
    public async Task<List<HabitEntry>> GetHabitEntriesAsync(int userId, int habitId, CancellationToken cancellationToken = default)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);

        if (habit == null)
            return new List<HabitEntry>();

        return await _context.HabitEntries
            .Where(he => he.HabitId == habitId)
            .OrderBy(he => he.Date)
            .ToListAsync(cancellationToken);
    }
} 