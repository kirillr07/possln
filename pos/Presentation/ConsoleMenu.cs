namespace Pos.Presentation;
using System.Globalization;
using Pos.Application.Ports.In;

public class ConsoleMenu
{
    private readonly IGradeSystemFacade _facade;
    private readonly ConsolePresenter _presenter;

    public ConsoleMenu(IGradeSystemFacade facade, ConsolePresenter presenter)
    {
        _facade = facade ?? throw new ArgumentNullException(nameof(facade));
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== Модуль оценивания «ПОС» ===");
            Console.WriteLine("1. Показать рубрикатор курса");
            Console.WriteLine("2. Добавить критерий в рубрикатор");
            Console.WriteLine("3. Показать список студентов");
            Console.WriteLine("4. Зарегистрировать студента");
            Console.WriteLine("5. Выставить / исправить оценку");
            Console.WriteLine("6. Отменить (удалить) оценку");
            Console.WriteLine("7. Показать ведомость студента (расчет итога)");
            Console.WriteLine("8. Показать сводную ведомость курса");
            Console.WriteLine("9. Экспорт ведомости в CSV");
            Console.WriteLine("10. Демонстрационный сценарий");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите пункт меню: ");

            var choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ShowRubric();
                        break;
                    case "2":
                        AddCriterion();
                        break;
                    case "3":
                        ShowStudents();
                        break;
                    case "4":
                        RegisterStudent();
                        break;
                    case "5":
                        SetEvaluation();
                        break;
                    case "6":
                        RemoveEvaluation();
                        break;
                    case "7":
                        ShowStudentReport();
                        break;
                    case "8":
                        ShowAllReports();
                        break;
                    case "9":
                        ExportGrades();
                        break;
                    case "10":
                        RunDemoScenario();
                        break;
                    case "0":
                        _presenter.ShowInfo("Работа программы завершена.");
                        return;
                    default:
                        _presenter.ShowError("Неизвестный пункт меню. Попробуйте снова.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _presenter.ShowError(ex.Message);
            }
        }
    }

    private void ShowRubric()
    {
        var rubric = _facade.GetRubric();
        _presenter.DisplayRubric(rubric);
    }

    private void AddCriterion()
    {
        Console.Write("Введите ID критерия (например, HW1, Test, Exam): ");
        var id = Console.ReadLine() ?? "";

        Console.Write("Введите название критерия: ");
        var name = Console.ReadLine() ?? "";

        Console.Write("Введите максимальный балл (например, 10 или 100): ");
        var maxScoreInput = Console.ReadLine()?.Replace(',', '.');
        if (!decimal.TryParse(maxScoreInput, NumberStyles.Any, CultureInfo.InvariantCulture, out var maxScore))
        {
            _presenter.ShowError("Некорректный формат числа для максимального балла.");
            return;
        }

        Console.Write("Введите вес критерия от 0.0 до 1.0 (например, 0.3 для 30%): ");
        var weightInput = Console.ReadLine()?.Replace(',', '.');
        if (!decimal.TryParse(weightInput, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            _presenter.ShowError("Некорректный формат числа для веса.");
            return;
        }

        _facade.AddCriterion(id, name, maxScore, weight);
        _presenter.ShowSuccess($"Критерий '{name}' успешно добавлен в рубрикатор.");
    }

    private void ShowStudents()
    {
        var students = _facade.GetAllStudents();
        _presenter.DisplayStudents(students);
    }

    private void RegisterStudent()
    {
        Console.Write("Введите ФИО студента: ");
        var fullName = Console.ReadLine() ?? "";

        var studentId = _facade.RegisterStudent(fullName);
        _presenter.ShowSuccess($"Студент зарегистрирован с ID: {studentId}");
    }

    private void SetEvaluation()
    {
        Console.Write("Введите ID студента (Guid): ");
        if (!Guid.TryParse(Console.ReadLine(), out var studentId))
        {
            _presenter.ShowError("Неверный формат GUID идентификатора студента.");
            return;
        }

        Console.Write("Введите ID критерия: ");
        var criterionId = Console.ReadLine() ?? "";

        Console.Write("Введите оценку: ");
        var scoreInput = Console.ReadLine()?.Replace(',', '.');
        if (!decimal.TryParse(scoreInput, NumberStyles.Any, CultureInfo.InvariantCulture, out var score))
        {
            _presenter.ShowError("Некорректный формат числа для оценки.");
            return;
        }

        Console.Write("Комментарий (опционально, нажмите Enter для пропуска): ");
        var comment = Console.ReadLine();

        _facade.SetStudentEvaluation(studentId, criterionId, score, comment);
        _presenter.ShowSuccess("Оценка успешно выставлена/обновлена.");
    }

    private void RemoveEvaluation()
    {
        Console.Write("Введите ID студента (Guid): ");
        if (!Guid.TryParse(Console.ReadLine(), out var studentId))
        {
            _presenter.ShowError("Неверный формат GUID идентификатора студента.");
            return;
        }

        Console.Write("Введите ID критерия для отмены: ");
        var criterionId = Console.ReadLine() ?? "";

        _facade.RemoveStudentEvaluation(studentId, criterionId);
        _presenter.ShowSuccess($"Оценка по критерию '{criterionId}' успешно удалена.");
    }

    private void ShowStudentReport()
    {
        Console.Write("Введите ID студента (Guid): ");
        if (!Guid.TryParse(Console.ReadLine(), out var studentId))
        {
            _presenter.ShowError("Неверный формат GUID идентификатора студента.");
            return;
        }

        var report = _facade.GetStudentReport(studentId);
        _presenter.DisplayReport(report);
    }

    private void ShowAllReports()
    {
        var reports = _facade.GetAllReports();
        _presenter.DisplayAllReports(reports);
    }

    private void ExportGrades()
    {
        Console.Write("Введите путь для экспорта (по умолчанию 'grades_export.csv'): ");
        var path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            path = "grades_export.csv";
        }

        _facade.ExportGrades(path);
        _presenter.ShowSuccess($"Ведомость успешно экспортирована в файл: {Path.GetFullPath(path)}");
    }

    private void RunDemoScenario()
    {
        _presenter.ShowInfo(">>> Запуск сквозного сценария ТЗ (Студенты -> Критерии -> Оценки -> Итог -> Экспорт) <<<");
        _facade.AddCriterion("HW", "Домашние задания", 100m, 0.4m);
        _facade.AddCriterion("TEST", "Промежуточный тест", 20m, 0.2m);
        _facade.AddCriterion("EXAM", "Итоговый экзамен", 10m, 0.4m);
        _presenter.ShowSuccess("1. Критерии рубрикатора успешно добавлены (HW: 40%, TEST: 20%, EXAM: 40%).");
        var petrId = _facade.RegisterStudent("Петр Петров");
        var ivanId = _facade.RegisterStudent("Иван Иванов");
        _presenter.ShowSuccess($"2. Студенты зарегистрированы:\n   - Петр ({petrId})\n   - Иван ({ivanId})");
        _facade.SetStudentEvaluation(petrId.Value, "HW", 95m, "Отличная работа по архитектуре");
        _facade.SetStudentEvaluation(petrId.Value, "TEST", 18m, "1 ошибка в теории");
        _facade.SetStudentEvaluation(petrId.Value, "EXAM", 9m, "Уверенная защита");
        _facade.SetStudentEvaluation(ivanId.Value, "HW", 60m, "Сдано с опозданием");
        _facade.SetStudentEvaluation(ivanId.Value, "TEST", 10m, "Половина решена");
        _presenter.ShowSuccess("3. Оценки выставлены в соответствии с инвариантами.");
        _presenter.ShowInfo("\n--- Проверка доменных инвариантов ---");
        try
        {
            _presenter.ShowInfo("Попытка поставить 12 баллов из 10 на экзамене...");
            _facade.SetStudentEvaluation(petrId.Value, "EXAM", 12m);
        }
        catch (Exception ex)
        {
            _presenter.ShowSuccess($"Инвариант сработал корректно: {ex.Message}");
        }

        try
        {
            _presenter.ShowInfo("Попытка оценить несуществующий критерий 'UNKNOWN'...");
            _facade.SetStudentEvaluation(petrId.Value, "UNKNOWN", 5m);
        }
        catch (Exception ex)
        {
            _presenter.ShowSuccess($"Инвариант сработал корректно: {ex.Message}");
        }
        _presenter.ShowInfo("\n--- 4. Расчет итога по формуле весов ---");
        var petrReport = _facade.GetStudentReport(petrId.Value);
        _presenter.DisplayReport(petrReport);
        const string exportFile = "demo_grades.csv";
        _facade.ExportGrades(exportFile);
        _presenter.ShowSuccess($"5. Итоговая ведомость выгружена в файл '{Path.GetFullPath(exportFile)}'.");
    }
}