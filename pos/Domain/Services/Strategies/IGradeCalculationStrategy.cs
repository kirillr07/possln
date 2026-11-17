namespace Pos.Domain.Services.Strategies;

using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;

public interface IGradeCalculationStrategy
{
    FinalGrade Calculate(Student student, Rubric rubric);
}