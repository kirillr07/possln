namespace Pos.Infrastructure.Persistence;

using System.Collections.Concurrent;
using Pos.Domain.Model;
using Pos.Domain.Ports;

public class InMemoryRubricRepository : IRubricRepository
{
    private readonly ConcurrentDictionary<Guid, Rubric> _rubrics = new();
    private Guid? _defaultRubricId;

    public Rubric? FindDefault()
    {
        if (_defaultRubricId.HasValue && _rubrics.TryGetValue(_defaultRubricId.Value, out var defaultRubric))
        {
            return defaultRubric;
        }
        
        return _rubrics.Values.FirstOrDefault();
    }

    public void Add(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        if (!_rubrics.TryAdd(rubric.Id, rubric))
        {
            throw new InvalidOperationException($"Рубрикатор с идентификатором '{rubric.Id}' уже существует.");
        }

        _defaultRubricId ??= rubric.Id;
    }

    public void Update(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        if (!_rubrics.ContainsKey(rubric.Id))
        {
            throw new KeyNotFoundException($"Рубрикатор с идентификатором '{rubric.Id}' не найден для обновления.");
        }

        _rubrics[rubric.Id] = rubric;
    }
}