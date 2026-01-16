using ModelLayer.Entities;
using DataLogic.Data;
using DataLogic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataLogic.Repositories;

public class LabelRepository(ApplicationDbContext dbContext): ILabelRepository
{
    public async Task<Label> CreateLabelAsync(Label label)
    {
        dbContext.Add(label);
        await dbContext.SaveChangesAsync();
        return label;
    }

    public async Task<IEnumerable<Label>> GetLabelsByUserIdAsync(int userId)
    {
        var query =  dbContext.Labels.AsQueryable()
            .Where(l => l.UserId == userId);

        return await query
            .Include(l => l.Notes)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Note>> GetNotesByLabelIdAsync(int labelId, int userId)
    {
        var label = await dbContext.Labels
            .Include(l => l.Notes)
                .ThenInclude(n => n.Labels)
            .Include(l => l.Notes)
                .ThenInclude(n => n.Collaborators)
            .FirstOrDefaultAsync(l => l.LabelId == labelId && l.UserId == userId);
        
        return label?.Notes ?? Enumerable.Empty<Note>();
    }

    public async Task<Label> UpdateLabelAsync(Label label)
    {
        var existingLabel = await dbContext.Labels.FirstOrDefaultAsync(l => l.LabelId == label.LabelId &&  l.UserId == label.UserId);

        if (existingLabel == null) return null;

        existingLabel.LabelName = label.LabelName;
        existingLabel.ChangedAt = DateTime.UtcNow;

        dbContext.Labels.Update(existingLabel);
        await dbContext.SaveChangesAsync();
        return existingLabel;
    }

    public async Task<bool> DeleteLabelAsync(int labelId, int userId)
    {
        var label = await dbContext.Labels.FirstOrDefaultAsync(l => l.LabelId == labelId && l.UserId == userId);
        
        
        if (label == null) return false;

        dbContext.Labels.Remove(label);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LabelExistsAsync(int labelId, int userId)
    {
        return await dbContext.Labels.AnyAsync(l => l.LabelId == labelId && l.UserId == userId);
    }
}