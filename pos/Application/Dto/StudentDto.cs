namespace Pos.Application.Dto;

public record StudentDto(
    Guid Id,
    string FullName,
    int EvaluationsCount);
