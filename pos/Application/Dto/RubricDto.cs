namespace Pos.Application.Dto;

public record CriterionDto(
    string Id,
    string Name,
    decimal MaxScore,
    decimal Weight);

public record RubricDto(
    string Name,
    IReadOnlyList<CriterionDto> Criteria);
