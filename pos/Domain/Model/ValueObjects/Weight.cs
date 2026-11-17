namespace Pos.Domain.Model.ValueObjects;

public readonly record struct Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value < 0 || value > 1.0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Вес критерия должен находиться в диапазоне [0, 1]. Получено: {value}.");
        }

        Value = value;
    }

    public override string ToString() => Value.ToString("P0");
}
