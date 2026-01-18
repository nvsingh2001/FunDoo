using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface ILabelService
{
    Task<LabelResponseDto> CreateLabelAsync(int userId, LabelRequestDto labelDto);
    Task<IEnumerable<LabelResponseDto>> GetAllLabelsAsync(int userId);
    Task<IEnumerable<NoteResponseDto>> GetNotesByLabelAsync(int userId, int labelId);
    Task<LabelResponseDto> UpdateLabelAsync(int userId, int labelId, LabelRequestDto labelDto);
    Task DeleteLabelAsync(int userId, int labelId);
}
