namespace Pos.Domain.Factories;

using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;

public class EvaluationFactory
{
    public Evaluation CreateEvaluation(
        Rubric rubric,
        CriterionId criterionId,
        decimal rawScore,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        var criterion = rubric.FindCriterion(criterionId);
        if (criterion is null)
        {
            throw new InvalidOperationException(
                $"Невозможно выставить оценку: критерий '{criterionId}' отсутствует в рубрикаторе '{rubric.Name}'.");
        }

        var score = new Score(rawScore, criterion.MaxScore);

        return new Evaluation(criterion.Id, score, comment);
    }
}
