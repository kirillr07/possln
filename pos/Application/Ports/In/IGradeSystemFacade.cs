namespace Pos.Application.Ports.In;
using Pos.Application.Dto;
using Pos.Domain.Model.ValueObjects;

public interface IGradeSystemFacade
{
    StudentId RegisterStudent(string fullName);
    IReadOnlyCollection<StudentDto> GetAllStudents();

    void AddCriterion(string id, string name, decimal maxScore, decimal rawWeight);
    RubricDto GetRubric();

    void SetStudentEvaluation(Guid studentId, string criterionId, decimal score, string? comment = null);
    void RemoveStudentEvaluation(Guid studentId, string criterionId);

    FinalGradeReportDto GetStudentReport(Guid studentId);
    IReadOnlyCollection<FinalGradeReportDto> GetAllReports();

    void ExportGrades(string destinationPath);
}