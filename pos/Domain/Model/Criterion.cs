namespace Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;

public class Criterion
{
    public CriterionId Id { get; }
    public string Name { get; }
    public decimal MaxScore { get; }
    public Weight Weight { get; }

    public Criterion(CriterionId id, string name, decimal maxScore, Weight weight)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Название критерия не может быть пустым.", nameof(name));
        }

        if (maxScore <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxScore), "Максимальный балл критерия должен быть строго больше 0.");
        }

        Id = id;
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }
}
