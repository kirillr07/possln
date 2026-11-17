namespace Pos.Tests.Domain;

using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Xunit;

public class EvaluationFactoryTests
{
    private readonly EvaluationFactory _factory = new();

    [Fact]
    public void CreateEvaluation_WhenCriterionExistsInRubric_ShouldSucceed()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW1"), "Домашка 1", 10m, new Weight(0.5m)));
        var evaluation = _factory.CreateEvaluation(rubric, new CriterionId("HW1"), 9.5m, "Хорошая работа");
        Assert.NotNull(evaluation);
        Assert.Equal(new CriterionId("HW1"), evaluation.CriterionId);
        Assert.Equal(9.5m, evaluation.Score.Value);
        Assert.Equal("Хорошая работа", evaluation.Comment);
    }

    [Fact]
    public void CreateEvaluation_WhenCriterionNotInRubric_ShouldThrowInvalidOperationException()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW1"), "Домашка 1", 10m, new Weight(0.5m)));
        var exception = Assert.Throws<InvalidOperationException>(() =>
            _factory.CreateEvaluation(rubric, new CriterionId("EXAM"), 8m));
        Assert.Contains("отсутствует в рубрикаторе", exception.Message);
    }

    [Fact]
    public void CreateEvaluation_ShouldIgnoreCaseAndWhitespaceInCriterionId()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW1"), "Домашка 1", 10m, new Weight(0.5m)));

        var evaluation = _factory.CreateEvaluation(rubric, new CriterionId("  hw1  "), 7m);

        Assert.Equal(new CriterionId("HW1"), evaluation.CriterionId);
    }

    [Fact]
    public void CreateEvaluation_WhenScoreAboveCriterionMax_ShouldThrow()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW1"), "Домашка 1", 10m, new Weight(0.5m)));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _factory.CreateEvaluation(rubric, new CriterionId("HW1"), 11m));
    }
}
