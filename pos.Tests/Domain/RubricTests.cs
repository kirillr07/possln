namespace Pos.Tests.Domain;

using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Xunit;

public class RubricTests
{
    [Fact]
    public void AddCriterion_WhenSumOfWeightsExceedsOne_ShouldAllow()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("A"), "Критерий A", 10m, new Weight(0.6m)));
        rubric.AddCriterion(new Criterion(new CriterionId("B"), "Критерий B", 10m, new Weight(0.5m)));

        Assert.Equal(2, rubric.Criteria.Count);
    }

    [Fact]
    public void AddCriterion_WhenAddingSameCriterionInDifferentCase_ShouldThrow()
    {
        var rubric = new Rubric(Guid.NewGuid(), "КПО");
        rubric.AddCriterion(new Criterion(new CriterionId("HW"), "Домашка", 10m, new Weight(0.5m)));

        Assert.Throws<InvalidOperationException>(() =>
            rubric.AddCriterion(new Criterion(new CriterionId(" hw "), "Домашка (дубль)", 10m, new Weight(0.5m))));
    }
}
