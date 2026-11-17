namespace Pos.Domain.Services.Strategies;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;

public class WeightedGradeStrategy : IGradeCalculationStrategy
{
    private const decimal TargetScale = FinalGrade.MaxValue;

    public FinalGrade Calculate(Student student, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);

        if (rubric.Criteria.Count == 0)
        {
            return new FinalGrade(0m);
        }
        decimal weightedGrade = 0m;
        foreach (var criterion in rubric.Criteria)
        {
            var evaluation = student.FindEvaluation(criterion.Id);
            if (evaluation is null)
            {
                continue;
            }

            decimal normalizedScore = (evaluation.Score.Value / criterion.MaxScore) * TargetScale;
            weightedGrade += normalizedScore * criterion.Weight.Value;
        }

        return new FinalGrade(weightedGrade);
    }
}