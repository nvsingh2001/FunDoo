using ModelLayer.Entities;

namespace DataLogic.Interfaces;

public interface ILabelRepository
{
    Task<Label> CreateLabelAsync(Label label);
    Task<IEnumerable<Label>> GetLabelsByUserIdAsync(int userId);
    Task<IEnumerable<Note>> GetNotesByLabelIdAsync(int labelId, int userId);
    Task<Label> UpdateLabelAsync(Label label);
    Task<bool> DeleteLabelAsync(int labelId, int userId);
    Task<bool> LabelExistsAsync(int labelId, int userId);
}
