using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace BusinessLogicLayer.Interfaces;

public interface ILabelService
{
    Task<ApiResponse<LabelResponseDto>> CreateLabelAsync(int userId, LabelRequestDto labelDto);
    Task<ApiResponse<IEnumerable<LabelResponseDto>>> GetAllLabelsAsync(int userId);
    Task<ApiResponse<IEnumerable<NoteResponseDto>>> GetNotesByLabelAsync(int userId, int labelId);
    Task<ApiResponse<LabelResponseDto>> UpdateLabelAsync(int userId, int labelId, LabelRequestDto labelDto);
    Task<ApiResponse<string>> DeleteLabelAsync(int userId, int labelId);
}
