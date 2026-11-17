namespace Pos.Domain.Model;

using Pos.Domain.Model.ValueObjects;

public class Student
{
    private readonly Dictionary<CriterionId, Evaluation> _evaluations = new();

    public StudentId Id { get; }
    public string FullName { get; private set; }

    public IReadOnlyCollection<Evaluation> Evaluations => _evaluations.Values;

    public Student(StudentId id, string fullName)
    {
        if (id.Value == Guid.Empty)
        {
            throw new ArgumentException("Идентификатор студента не может быть пустым.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Имя студента не может быть пустым.", nameof(fullName));
        }

        Id = id;
        FullName = fullName.Trim();
    }

    public void SetEvaluation(Criterion criterion, Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(criterion);
        ArgumentNullException.ThrowIfNull(evaluation);

        if (criterion.Id != evaluation.CriterionId)
        {
            throw new ArgumentException(
                $"Оценка относится к критерию '{evaluation.CriterionId}', а не к '{criterion.Id}'.",
                nameof(evaluation));
        }

        if (evaluation.Score.Value < 0 || evaluation.Score.Value > criterion.MaxScore)
        {
            throw new ArgumentOutOfRangeException(
                nameof(evaluation),
                $"Балл ({evaluation.Score.Value}) выходит за допустимый диапазон критерия [0, {criterion.MaxScore}].");
        }

        _evaluations[criterion.Id] = evaluation;
    }

    public bool RemoveEvaluation(CriterionId criterionId) =>
        _evaluations.Remove(criterionId);

    public Evaluation? FindEvaluation(CriterionId criterionId) =>
        _evaluations.GetValueOrDefault(criterionId);
}