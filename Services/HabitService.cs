using Microsoft.EntityFrameworkCore;
using MinimalHabitsApi.Data;
using MinimalHabitsApi.DTOs;
using MinimalHabitsApi.Models;

namespace MinimalHabitsApi.Services;

public class HabitService
{
    private readonly ApplicationDbContext _context;

    public HabitService(ApplicationDbContext context)
    {
        _context = context;
    }

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

    public async Task<List<Habit>> GetUserHabitsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Entries)
            .ToListAsync(cancellationToken);
    }

    public async Task<Habit?> GetHabitAsync(int userId, int habitId, CancellationToken cancellationToken = default)
    {
        return await _context.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId, cancellationToken);
    }

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