namespace Pos.Domain.Model.ValueObjects;

public readonly record struct FinalGrade
{
    public const decimal MaxValue = 10m;

    public decimal Value { get; }

    public FinalGrade(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Итоговый балл не может быть отрицательным. Получено: {value}.");
        }

        var rounded = Math.Round(value, 2, MidpointRounding.AwayFromZero);
        Value = Math.Min(rounded, MaxValue);
    }

    public override string ToString() => Value.ToString("0.00");
}
