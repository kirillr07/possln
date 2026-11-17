namespace Pos.Domain.Model.ValueObjects;

public readonly record struct Score
{
    public decimal Value { get; }

    public Score(decimal value, decimal maxScore)
    {
        if (maxScore <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxScore), "Максимальный балл критерия должен быть строго больше 0.");
        }

        if (value < 0 || value > maxScore)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Балл ({value}) выходит за допустимый диапазон критерия [0, {maxScore}].");
        }

        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}
