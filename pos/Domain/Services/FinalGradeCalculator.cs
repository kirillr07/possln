namespace Pos.Domain.Services;

using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Pos.Domain.Services.Strategies;

public class FinalGradeCalculator
{
    private readonly IGradeCalculationStrategy _calculationStrategy;

    public FinalGradeCalculator(IGradeCalculationStrategy calculationStrategy)
    {
        _calculationStrategy = calculationStrategy ?? throw new ArgumentNullException(nameof(calculationStrategy));
    }

    public FinalGrade Calculate(Student student, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);

        return _calculationStrategy.Calculate(student, rubric);
    }
}