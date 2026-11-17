namespace Pos.Tests.Domain;

using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Xunit;

public class StudentTests
{
    private readonly EvaluationFactory _factory = new();

    [Fact]
    public void SetEvaluation_WithScoreAboveCriterionMax_ShouldThrow()
    {
        var exam = new Criterion(new CriterionId("EXAM"), "Экзамен", 10m, new Weight(1m));
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(exam);

        var looseCriterion = new Criterion(new CriterionId("EXAM"), "Экзамен", 1000m, new Weight(1m));
        var looseRubric = new Rubric(Guid.NewGuid(), "Другой рубрикатор");
        looseRubric.AddCriterion(looseCriterion);

        var forgedEvaluation = _factory.CreateEvaluation(looseRubric, looseCriterion.Id, 999m);
        var student = new Student(StudentId.New(), "Тест");

        Assert.Throws<ArgumentOutOfRangeException>(() => student.SetEvaluation(exam, forgedEvaluation));
    }

    [Fact]
    public void SetEvaluation_WithDifferentCriterionId_ShouldThrow()
    {
        var exam = new Criterion(new CriterionId("EXAM"), "Экзамен", 10m, new Weight(0.5m));
        var other = new Criterion(new CriterionId("OTHER"), "Другой критерий", 10m, new Weight(0.5m));
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(exam);
        rubric.AddCriterion(other);

        var evaluation = _factory.CreateEvaluation(rubric, other.Id, 5m);
        var student = new Student(StudentId.New(), "Тест");

        Assert.Throws<ArgumentException>(() => student.SetEvaluation(exam, evaluation));
    }

    [Fact]
    public void SetEvaluation_WhenCalledTwice_ShouldReplacePreviousEvaluation()
    {
        var homework = new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(1m));
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(homework);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(homework, _factory.CreateEvaluation(rubric, homework.Id, 5m));
        student.SetEvaluation(homework, _factory.CreateEvaluation(rubric, homework.Id, 9m));

        Assert.Single(student.Evaluations);
        Assert.Equal(9m, student.FindEvaluation(new CriterionId("HW"))!.Score.Value);
    }

    [Fact]
    public void FindEvaluation_ShouldIgnoreCaseAndWhitespace()
    {
        var homework = new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(1m));
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(homework);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(homework, _factory.CreateEvaluation(rubric, homework.Id, 7m));

        var found = student.FindEvaluation(new CriterionId("  hw  "));

        Assert.NotNull(found);
        Assert.Equal(7m, found.Score.Value);
    }

    [Fact]
    public void RemoveEvaluation_ShouldIgnoreCaseAndWhitespace()
    {
        var homework = new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(1m));
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(homework);

        var student = new Student(StudentId.New(), "Тест");
        student.SetEvaluation(homework, _factory.CreateEvaluation(rubric, homework.Id, 7m));

        var removed = student.RemoveEvaluation(new CriterionId(" hw "));

        Assert.True(removed);
        Assert.Empty(student.Evaluations);
    }
}
