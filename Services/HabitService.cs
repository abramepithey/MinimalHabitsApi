using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.DTOs;
using MinimalApi.Models;

namespace MinimalApi.Services;

public class HabitService
{
    private readonly ApplicationDbContext _context;

    public HabitService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Habit> CreateHabitAsync(int userId, HabitDto habitDto)
    {
        var habit = new Habit
        {
            Name = habitDto.Name,
            UserId = userId
        };

        _context.Habits.Add(habit);
        await _context.SaveChangesAsync();
        return habit;
    }

    public async Task<List<Habit>> GetUserHabitsAsync(int userId)
    {
        return await _context.Habits
            .Where(h => h.UserId == userId)
            .Include(h => h.Entries)
            .ToListAsync();
    }

    public async Task<Habit?> GetHabitAsync(int userId, int habitId)
    {
        return await _context.Habits
            .Include(h => h.Entries)
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
    }

    public async Task<bool> UpdateHabitAsync(int userId, int habitId, HabitDto habitDto)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

        if (habit == null)
            return false;

        habit.Name = habitDto.Name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHabitAsync(int userId, int habitId)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

        if (habit == null)
            return false;

        _context.Habits.Remove(habit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<HabitEntry?> AddHabitEntryAsync(int userId, int habitId, HabitEntryDto entryDto)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

        if (habit == null)
            return null;

        var entry = new HabitEntry
        {
            Date = entryDto.Date,
            Completed = entryDto.Completed,
            HabitId = habitId
        };

        _context.HabitEntries.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task<List<HabitEntry>> GetHabitEntriesAsync(int userId, int habitId)
    {
        var habit = await _context.Habits
            .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

        if (habit == null)
            return new List<HabitEntry>();

        return await _context.HabitEntries
            .Where(he => he.HabitId == habitId)
            .OrderBy(he => he.Date)
            .ToListAsync();
    }
} 