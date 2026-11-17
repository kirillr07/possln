namespace Pos.Domain.Model;

using Pos.Domain.Model.ValueObjects;

public class Evaluation
{
    public CriterionId CriterionId { get; }
    public Score Score { get; }
    public DateTime EvaluatedAt { get; }
    public string? Comment { get; }

    internal Evaluation(CriterionId criterionId, Score score, string? comment = null, DateTime? evaluatedAt = null)
    {
        CriterionId = criterionId;
        Score = score;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        EvaluatedAt = evaluatedAt ?? DateTime.UtcNow;
    }
}
