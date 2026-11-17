namespace Pos.Presentation;

using Pos.Application.Dto;

public class ConsolePresenter
{
    public void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    public void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Ошибка: {message}");
        Console.ResetColor();
    }

    public void ShowInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void DisplayStudents(IReadOnlyCollection<StudentDto> students)
    {
        if (students.Count == 0)
        {
            Console.WriteLine("Список студентов пуст.");
            return;
        }

        Console.WriteLine("\n--- Список студентов ---");
        Console.WriteLine($"{"ID",-38} | {"ФИО",-25} | {"Выставлено оценок",-18}");
        Console.WriteLine(new string('-', 88));

        foreach (var student in students)
        {
            Console.WriteLine($"{student.Id,-38} | {student.FullName,-25} | {student.EvaluationsCount,-18}");
        }
    }

    public void DisplayRubric(RubricDto rubric)
    {
        Console.WriteLine($"\n--- Рубрикатор: {rubric.Name} ---");
        if (rubric.Criteria.Count == 0)
        {
            Console.WriteLine("Критерии еще не добавлены.");
            return;
        }

        Console.WriteLine($"{"ID",-12} | {"Название критерия",-30} | {"Макс. балл",-12} | {"Вес",-8}");
        Console.WriteLine(new string('-', 70));

        foreach (var c in rubric.Criteria)
        {
            Console.WriteLine($"{c.Id,-12} | {c.Name,-30} | {c.MaxScore,-12:0.##} | {c.Weight,-8}");
        }
    }

    public void DisplayReport(FinalGradeReportDto report)
    {
        Console.WriteLine($"\n========================================================");
        Console.WriteLine($"Ведомость студента: {report.FullName}");
        Console.WriteLine($"ID: {report.StudentId}");
        Console.WriteLine($"Итоговый балл (по весам): {report.FinalGrade:0.00} из 10.00");
        Console.WriteLine($"--------------------------------------------------------");
        Console.WriteLine($"{"Критерий",-25} | {"Макс.",-6} | {"Вес",-6} | {"Оценка",-8} | {"Комментарий"}");
        Console.WriteLine(new string('-', 68));

        foreach (var d in report.Details)
        {
            var scoreText = d.AwardedScore.HasValue ? d.AwardedScore.Value.ToString("0.##") : "-";
            var commentText = string.IsNullOrWhiteSpace(d.Comment) ? "" : d.Comment;
            Console.WriteLine($"{d.CriterionName,-25} | {d.MaxScore,-6:0.##} | {d.Weight,-6:P0} | {scoreText,-8} | {commentText}");
        }
        Console.WriteLine($"========================================================\n");
    }

    public void DisplayAllReports(IReadOnlyCollection<FinalGradeReportDto> reports)
    {
        if (reports.Count == 0)
        {
            Console.WriteLine("Нет данных для отображения сводной ведомости.");
            return;
        }

        Console.WriteLine("\n--- Сводная итоговая ведомость курса ---");
        Console.WriteLine($"{"ФИО Студента",-28} | {"Итоговый балл",-15}");
        Console.WriteLine(new string('-', 46));

        foreach (var r in reports)
        {
            Console.WriteLine($"{r.FullName,-28} | {r.FinalGrade,-15:0.00}");
        }
    }
}