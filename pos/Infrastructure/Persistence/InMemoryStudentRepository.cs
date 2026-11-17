namespace Pos.Infrastructure.Persistence;

using System.Collections.Concurrent;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Pos.Domain.Ports;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly ConcurrentDictionary<StudentId, Student> _students = new();

    public Student? FindById(StudentId id)
    {
        _students.TryGetValue(id, out var student);
        return student;
    }

    public IReadOnlyCollection<Student> GetAll() => _students.Values.ToList();

    public void Add(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (!_students.TryAdd(student.Id, student))
        {
            throw new InvalidOperationException($"Студент с идентификатором '{student.Id}' уже зарегистрирован в системе.");
        }
    }

    public void Update(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        if (!_students.ContainsKey(student.Id))
        {
            throw new KeyNotFoundException($"Студент с идентификатором '{student.Id}' не найден для обновления.");
        }

        _students[student.Id] = student;
    }
}