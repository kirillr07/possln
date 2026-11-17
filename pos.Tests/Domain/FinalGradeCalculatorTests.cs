namespace Pos.Tests.Domain;

using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Pos.Domain.Services;
using Pos.Domain.Services.Strategies;
using Xunit;

public class FinalGradeCalculatorTests
{
    private readonly FinalGradeCalculator _calculator;
    private readonly EvaluationFactory _factory = new();

    public FinalGradeCalculatorTests()
    {
        _calculator = new FinalGradeCalculator(new WeightedGradeStrategy());
    }

    [Fact]
    public void Calculate_WithWeightedCriteria_ComputesCorrectFinalGrade()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var hw = new Criterion(new CriterionId("HW"), "Домашнее задание", 100m, new Weight(0.4m));
        var exam = new Criterion(new CriterionId("EXAM"), "Экзамен", 10m, new Weight(0.6m));
        rubric.AddCriterion(hw);
        rubric.AddCriterion(exam);

        var student = new Student(StudentId.New(), "Петр Петров");
        student.SetEvaluation(hw, _factory.CreateEvaluation(rubric, hw.Id, 80m));
        student.SetEvaluation(exam, _factory.CreateEvaluation(rubric, exam.Id, 10m));

        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(9.20m, finalGrade.Value);
    }

    [Fact]
    public void Calculate_WhenNoEvaluationsSet_ReturnsZero()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(1.0m)));
        var student = new Student(StudentId.New(), "Иван Иванов");
        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(0m, finalGrade.Value);
    }

    [Fact]
    public void Calculate_WhenWeightsSumIsLessThanOne_ReturnsPartialMax()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var first = new Criterion(new CriterionId("A"), "Критерий A", 10m, new Weight(0.4m));
        var second = new Criterion(new CriterionId("B"), "Критерий B", 10m, new Weight(0.4m));
        rubric.AddCriterion(first);
        rubric.AddCriterion(second);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(first, _factory.CreateEvaluation(rubric, first.Id, 10m));
        student.SetEvaluation(second, _factory.CreateEvaluation(rubric, second.Id, 10m));

        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(8m, finalGrade.Value);
    }

    [Fact]
    public void Calculate_WhenWeightedSumExceedsTen_ClampsToTen()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var first = new Criterion(new CriterionId("A"), "Критерий A", 10m, new Weight(1m));
        var second = new Criterion(new CriterionId("B"), "Критерий B", 10m, new Weight(1m));
        rubric.AddCriterion(first);
        rubric.AddCriterion(second);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(first, _factory.CreateEvaluation(rubric, first.Id, 10m));
        student.SetEvaluation(second, _factory.CreateEvaluation(rubric, second.Id, 10m));

        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(10m, finalGrade.Value);
    }

    [Fact]
    public void Calculate_WhenAllWeightsAreZero_ReturnsZero()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var criterion = new Criterion(new CriterionId("A"), "Критерий A", 10m, new Weight(0m));
        rubric.AddCriterion(criterion);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(criterion, _factory.CreateEvaluation(rubric, criterion.Id, 10m));

        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(0m, finalGrade.Value);
    }

    [Fact]
    public void Calculate_WhenEvaluationIsMissing_CountsCriterionAsZero()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        var hw = new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(0.4m));
        var exam = new Criterion(new CriterionId("EXAM"), "Экзамен", 10m, new Weight(0.6m));
        rubric.AddCriterion(hw);
        rubric.AddCriterion(exam);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(hw, _factory.CreateEvaluation(rubric, hw.Id, 10m));

        var finalGrade = _calculator.Calculate(student, rubric);
        Assert.Equal(4m, finalGrade.Value);
    }
}
