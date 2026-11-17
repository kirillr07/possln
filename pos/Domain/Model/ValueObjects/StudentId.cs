namespace Pos.Domain.Model.ValueObjects;

public readonly record struct StudentId
{
    public Guid Value { get; }

    public StudentId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Идентификатор студента не может быть пустым (Guid.Empty).", nameof(value));
        }

        Value = value;
    }

    public static StudentId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
