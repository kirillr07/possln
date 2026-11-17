namespace Pos.Tests.Application;

using Pos.Application.Dto;
using Pos.Application.Facade;
using Pos.Application.Ports.Out;
using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Services;
using Pos.Domain.Services.Strategies;
using Pos.Infrastructure.Persistence;
using Xunit;

public class GradeSystemFacadeTests
{
    private readonly InMemoryStudentRepository _students = new();
    private readonly InMemoryRubricRepository _rubrics = new();
    private readonly FakeGradeExporter _exporter = new();
    private readonly GradeSystemFacade _facade;

    public GradeSystemFacadeTests()
    {
        _rubrics.Add(new Rubric(Guid.NewGuid(), "КПО"));
        _facade = new GradeSystemFacade(
            _students,
            _rubrics,
            new EvaluationFactory(),
            new FinalGradeCalculator(new WeightedGradeStrategy()),
            _exporter);
    }

    [Fact]
    public void FullScenario_ComputesFinalGradeByWeights()
    {
        _facade.AddCriterion("HW", "Домашняя работа", 100m, 0.4m);
        _facade.AddCriterion("EXAM", "Экзамен", 10m, 0.6m);

        var studentId = _facade.RegisterStudent("Алиса Селезнева");
        _facade.SetStudentEvaluation(studentId.Value, "HW", 100m);
        _facade.SetStudentEvaluation(studentId.Value, "EXAM", 8m);

        var report = _facade.GetStudentReport(studentId.Value);

        Assert.Equal(8.8m, report.FinalGrade);
        Assert.Equal(2, report.Details.Count);
    }

    [Fact]
    public void GetStudentReport_WhenWeightsExceedOne_ClampsFinalGradeToTen()
    {
        _facade.AddCriterion("HW", "Домашняя работа", 10m, 0.8m);
        _facade.AddCriterion("EXAM", "Экзамен", 10m, 0.8m);
        var studentId = _facade.RegisterStudent("Алиса Селезнева");
        _facade.SetStudentEvaluation(studentId.Value, "HW", 10m);
        _facade.SetStudentEvaluation(studentId.Value, "EXAM", 10m);

        var report = _facade.GetStudentReport(studentId.Value);

        Assert.Equal(10m, report.FinalGrade);
    }

    [Fact]
    public void SetStudentEvaluation_WhenCriterionDoesNotExist_ShouldThrow()
    {
        _facade.AddCriterion("HW", "Домашняя работа", 100m, 1m);
        var studentId = _facade.RegisterStudent("Иван Иванов");

        Assert.Throws<InvalidOperationException>(() =>
            _facade.SetStudentEvaluation(studentId.Value, "UNKNOWN", 5m));
    }

    [Fact]
    public void SetStudentEvaluation_WhenScoreExceedsCriterionMax_ShouldThrow()
    {
        _facade.AddCriterion("EXAM", "Экзамен", 10m, 1m);
        var studentId = _facade.RegisterStudent("Иван Иванов");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _facade.SetStudentEvaluation(studentId.Value, "EXAM", 12m));
    }

    [Fact]
    public void RemoveStudentEvaluation_ShouldExcludeCriterionFromFinalGrade()
    {
        _facade.AddCriterion("HW", "Домашняя работа", 10m, 0.5m);
        _facade.AddCriterion("EXAM", "Экзамен", 10m, 0.5m);
        var studentId = _facade.RegisterStudent("Петр Петров");
        _facade.SetStudentEvaluation(studentId.Value, "HW", 10m);
        _facade.SetStudentEvaluation(studentId.Value, "EXAM", 10m);

        _facade.RemoveStudentEvaluation(studentId.Value, "exam");

        var report = _facade.GetStudentReport(studentId.Value);
        var examDetail = Assert.Single(report.Details, d => d.CriterionId == "EXAM");
        Assert.Null(examDetail.AwardedScore);
        Assert.Equal(5m, report.FinalGrade);
    }

    [Fact]
    public void ExportGrades_ShouldPassReportsToExporter()
    {
        _facade.AddCriterion("HW", "Домашняя работа", 10m, 1m);
        var studentId = _facade.RegisterStudent("Алиса Селезнева");
        _facade.SetStudentEvaluation(studentId.Value, "HW", 9m);

        _facade.ExportGrades("grades.csv");

        Assert.Equal("grades.csv", _exporter.LastPath);
        Assert.NotNull(_exporter.LastReports);
        var report = Assert.Single(_exporter.LastReports);
        Assert.Equal("Алиса Селезнева", report.FullName);
        Assert.Equal(9m, report.FinalGrade);
    }

    [Fact]
    public void GetStudentReport_WhenStudentDoesNotExist_ShouldThrow()
    {
        Assert.Throws<KeyNotFoundException>(() => _facade.GetStudentReport(Guid.NewGuid()));
    }

    private sealed class FakeGradeExporter : IGradeExporter
    {
        public string? LastPath { get; private set; }
        public IReadOnlyCollection<FinalGradeReportDto>? LastReports { get; private set; }

        public void Export(IReadOnlyCollection<FinalGradeReportDto> reports, string destinationPath)
        {
            LastReports = reports;
            LastPath = destinationPath;
        }
    }
}
