namespace Pos.Domain.Model.ValueObjects;

public readonly record struct CriterionId
{
    public string Value { get; }

    public CriterionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Идентификатор критерия не может быть пустым.", nameof(value));
        }

        Value = value.Trim();
    }

    public bool Equals(CriterionId other) =>
        string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        Value is null ? 0 : StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value ?? string.Empty;
}
