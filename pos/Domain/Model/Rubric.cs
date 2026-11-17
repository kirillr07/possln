namespace Pos.Domain.Model;

using Pos.Domain.Model.ValueObjects;

public class Rubric
{
    private readonly Dictionary<CriterionId, Criterion> _criteria = new();

    public Guid Id { get; }
    public string Name { get; private set; }

    public IReadOnlyCollection<Criterion> Criteria => _criteria.Values;

    public Rubric(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Идентификатор рубрикатора не может быть пустым.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Название рубрикатора не может быть пустым.", nameof(name));
        }

        Id = id;
        Name = name.Trim();
    }

    public void AddCriterion(Criterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);

        if (_criteria.ContainsKey(criterion.Id))
        {
            throw new InvalidOperationException($"Критерий с идентификатором '{criterion.Id}' уже добавлен в рубрикатор.");
        }

        _criteria.Add(criterion.Id, criterion);
    }

    public Criterion? FindCriterion(CriterionId criterionId) =>
        _criteria.GetValueOrDefault(criterionId);

    public Criterion GetCriterion(CriterionId criterionId)
    {
        if (!_criteria.TryGetValue(criterionId, out var criterion))
        {
            throw new KeyNotFoundException($"Критерий с идентификатором '{criterionId}' не найден в рубрикаторе.");
        }

        return criterion;
    }
}
