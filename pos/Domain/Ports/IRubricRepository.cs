namespace Pos.Domain.Ports;
using Pos.Domain.Model;

public interface IRubricRepository
{
    Rubric? FindDefault();
    void Add(Rubric rubric);
    void Update(Rubric rubric);
}
