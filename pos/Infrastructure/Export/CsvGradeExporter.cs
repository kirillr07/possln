namespace Pos.Infrastructure.Export;

using System.Globalization;
using System.Text;
using Pos.Application.Dto;
using Pos.Application.Ports.Out;

public class CsvGradeExporter : IGradeExporter
{
    private const string Delimiter = ";";

    public void Export(IReadOnlyCollection<FinalGradeReportDto> reports, string destinationPath)
    {
        ArgumentNullException.ThrowIfNull(reports);

        if (string.IsNullOrWhiteSpace(destinationPath))
        {
            throw new ArgumentException("Путь к файлу экспорта не может быть пустым.", nameof(destinationPath));
        }

        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var criteriaHeaders = reports
            .SelectMany(r => r.Details)
            .GroupBy(d => d.CriterionId)
            .Select(g => g.First())
            .OrderBy(c => c.CriterionId)
            .ToList();

        var sb = new StringBuilder();

        var headerColumns = new List<string> { "StudentId", "ФИО Студента", "Итоговый балл" };
        foreach (var crit in criteriaHeaders)
        {
            headerColumns.Add(EscapeCsv($"{crit.CriterionName} (макс. {crit.MaxScore}, вес {crit.Weight:P0})"));
        }
        sb.AppendLine(string.Join(Delimiter, headerColumns));

        foreach (var report in reports)
        {
            var row = new List<string>
            {
                EscapeCsv(report.StudentId.ToString()),
                EscapeCsv(report.FullName),
                report.FinalGrade.ToString("0.00", CultureInfo.InvariantCulture)
            };

            foreach (var crit in criteriaHeaders)
            {
                var detail = report.Details.FirstOrDefault(d => d.CriterionId == crit.CriterionId);
                var scoreStr = detail?.AwardedScore.HasValue == true
                    ? detail.AwardedScore.Value.ToString("0.##", CultureInfo.InvariantCulture)
                    : "-";

                row.Add(scoreStr);
            }

            sb.AppendLine(string.Join(Delimiter, row));
        }

        File.WriteAllText(destinationPath, sb.ToString(), new UTF8Encoding(true));
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        return value;
    }
}