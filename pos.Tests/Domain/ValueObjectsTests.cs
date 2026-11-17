namespace Pos.Tests.Domain;

using Pos.Domain.Model.ValueObjects;
using Xunit;

public class ValueObjectsTests
{
    [Theory]
    [InlineData(0, 10)]
    [InlineData(5.5, 10)]
    [InlineData(10, 10)]
    public void Score_WithValidRange_ShouldCreateSuccessfully(decimal value, decimal max)
    {
        var score = new Score(value, max);
        Assert.Equal(value, score.Value);
    }

    [Fact]
    public void Score_ExceedingMax_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(11m, 10m));
    }

    [Fact]
    public void Score_BelowZero_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(-1m, 10m));
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.05)]
    public void Weight_OutsideZeroToOne_ShouldThrowArgumentOutOfRangeException(decimal weightValue)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Weight(weightValue));
    }

    [Fact]
    public void FinalGrade_AboveMax_ShouldClampToMax()
    {
        var grade = new FinalGrade(12m);

        Assert.Equal(FinalGrade.MaxValue, grade.Value);
    }

    [Fact]
    public void FinalGrade_AtMax_ShouldCreateSuccessfully()
    {
        var grade = new FinalGrade(FinalGrade.MaxValue);

        Assert.Equal(FinalGrade.MaxValue, grade.Value);
    }

    [Fact]
    public void FinalGrade_Negative_ShouldThrowArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FinalGrade(-1m));
    }

    [Fact]
    public void CriterionId_ShouldTrimValue()
    {
        var id = new CriterionId("  HW1  ");

        Assert.Equal("HW1", id.Value);
    }

    [Fact]
    public void CriterionId_ShouldIgnoreCaseInEquality()
    {
        var first = new CriterionId("hw1");
        var second = new CriterionId(" HW1 ");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CriterionId_WhenEmpty_ShouldThrowArgumentException(string value)
    {
        Assert.Throws<ArgumentException>(() => new CriterionId(value));
    }
}