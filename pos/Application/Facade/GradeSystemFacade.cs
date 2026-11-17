namespace Pos.Application.Facade;

using Pos.Application.Dto;
using Pos.Application.Ports.In;
using Pos.Application.Ports.Out;
using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Model.ValueObjects;
using Pos.Domain.Ports;
using Pos.Domain.Services;

public class GradeSystemFacade : IGradeSystemFacade
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRubricRepository _rubricRepository;
    private readonly EvaluationFactory _evaluationFactory;
    private readonly FinalGradeCalculator _gradeCalculator;
    private readonly IGradeExporter _gradeExporter;

    public GradeSystemFacade(
        IStudentRepository studentRepository,
        IRubricRepository rubricRepository,
        EvaluationFactory evaluationFactory,
        FinalGradeCalculator gradeCalculator,
        IGradeExporter gradeExporter)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _rubricRepository = rubricRepository ?? throw new ArgumentNullException(nameof(rubricRepository));
        _evaluationFactory = evaluationFactory ?? throw new ArgumentNullException(nameof(evaluationFactory));
        _gradeCalculator = gradeCalculator ?? throw new ArgumentNullException(nameof(gradeCalculator));
        _gradeExporter = gradeExporter ?? throw new ArgumentNullException(nameof(gradeExporter));
    }

    public StudentId RegisterStudent(string fullName)
    {
        var id = StudentId.New();
        var student = new Student(id, fullName);
        _studentRepository.Add(student);
        return id;
    }

    public IReadOnlyCollection<StudentDto> GetAllStudents() =>
        _studentRepository.GetAll()
            .Select(student => new StudentDto(
                student.Id.Value,
                student.FullName,
                student.Evaluations.Count))
            .ToList();

    public void AddCriterion(string id, string name, decimal maxScore, decimal rawWeight)
    {
        var rubric = GetActiveRubric();
        var criterion = new Criterion(new CriterionId(id), name, maxScore, new Weight(rawWeight));
        rubric.AddCriterion(criterion);
        _rubricRepository.Update(rubric);
    }

    public RubricDto GetRubric()
    {
        var rubric = GetActiveRubric();

        return new RubricDto(
            rubric.Name,
            rubric.Criteria
                .Select(criterion => new CriterionDto(
                    criterion.Id.Value,
                    criterion.Name,
                    criterion.MaxScore,
                    criterion.Weight.Value))
                .ToList());
    }

    public void SetStudentEvaluation(Guid studentId, string criterionId, decimal score, string? comment = null)
    {
        var student = GetStudentOrThrow(studentId);
        var rubric = GetActiveRubric();
        var evaluation = _evaluationFactory.CreateEvaluation(rubric, new CriterionId(criterionId), score, comment);
        var criterion = rubric.GetCriterion(evaluation.CriterionId);

        student.SetEvaluation(criterion, evaluation);
        _studentRepository.Update(student);
    }

    public void RemoveStudentEvaluation(Guid studentId, string criterionId)
    {
        var student = GetStudentOrThrow(studentId);
        var removed = student.RemoveEvaluation(new CriterionId(criterionId));

        if (!removed)
        {
            throw new InvalidOperationException($"У студента нет оценки по критерию '{criterionId}'.");
        }

        _studentRepository.Update(student);
    }

    public FinalGradeReportDto GetStudentReport(Guid studentId)
    {
        var student = GetStudentOrThrow(studentId);
        var rubric = GetActiveRubric();

        return BuildReport(student, rubric);
    }

    public IReadOnlyCollection<FinalGradeReportDto> GetAllReports()
    {
        var rubric = GetActiveRubric();
        var students = _studentRepository.GetAll();

        return students.Select(student => BuildReport(student, rubric)).ToList();
    }

    public void ExportGrades(string destinationPath)
    {
        var reports = GetAllReports();
        _gradeExporter.Export(reports, destinationPath);
    }

    private Rubric GetActiveRubric()
    {
        var rubric = _rubricRepository.FindDefault();
        if (rubric is null)
        {
            throw new InvalidOperationException("Рубрикатор курса не инициализирован.");
        }

        return rubric;
    }

    private Student GetStudentOrThrow(Guid studentId)
    {
        var id = new StudentId(studentId);
        var student = _studentRepository.FindById(id);

        if (student is null)
        {
            throw new KeyNotFoundException($"Студент с идентификатором '{studentId}' не найден.");
        }

        return student;
    }

    private FinalGradeReportDto BuildReport(Student student, Rubric rubric)
    {
        var finalGrade = _gradeCalculator.Calculate(student, rubric);

        var details = rubric.Criteria.Select(criterion =>
        {
            var evaluation = student.FindEvaluation(criterion.Id);
            return new CriterionGradeDetailDto(
                CriterionId: criterion.Id.Value,
                CriterionName: criterion.Name,
                MaxScore: criterion.MaxScore,
                Weight: criterion.Weight.Value,
                AwardedScore: evaluation?.Score.Value,
                Comment: evaluation?.Comment
            );
        }).ToList();

        return new FinalGradeReportDto(
            student.Id.Value,
            student.FullName,
            finalGrade.Value,
            details
        );
    }
}