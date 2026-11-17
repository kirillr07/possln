namespace Pos.Application.Dto;

public record CriterionGradeDetailDto(
    string CriterionId,
    string CriterionName,
    decimal MaxScore,
    decimal Weight,
    decimal? AwardedScore,
    string? Comment);

public record FinalGradeReportDto(
    Guid StudentId,
    string FullName,
    decimal FinalGrade,
    IReadOnlyList<CriterionGradeDetailDto> Details);