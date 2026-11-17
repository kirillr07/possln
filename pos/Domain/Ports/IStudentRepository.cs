namespace Pos.Domain.Ports;

using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;

public interface IStudentRepository
{
    Student? FindById(StudentId id);
    IReadOnlyCollection<Student> GetAll();
    void Add(Student student);
    void Update(Student student);
}
