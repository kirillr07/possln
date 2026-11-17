namespace Pos.Tests.Infrastructure;

using Pos.Application.Dto;
using Pos.Infrastructure.Export;
using Xunit;

public class CsvGradeExporterTests
{
    private readonly CsvGradeExporter _exporter = new();

    [Fact]
    public void Export_ShouldEscapeDelimitersAndQuotes()
    {
        var reports = new List<FinalGradeReportDto>
        {
            new(
                Guid.NewGuid(),
                "Иванов; Иван",
                8.5m,
                new List<CriterionGradeDetailDto>
                {
                    new("HW", "Домашка; \"важная\"", 100.5m, 0.5m, 90m, null)
                }),
            new(
                Guid.NewGuid(),
                "Петров Петр",
                0m,
                new List<CriterionGradeDetailDto>
                {
                    new("HW", "Домашка; \"важная\"", 100.5m, 0.5m, null, null)
                })
        };

        var path = Path.Combine(Path.GetTempPath(), $"pos-export-{Guid.NewGuid():N}.csv");

        try
        {
            _exporter.Export(reports, path);
            var text = File.ReadAllText(path);

            Assert.Contains("\"Иванов; Иван\"", text);
            Assert.Contains("\"Домашка; \"\"важная\"\"", text);
            Assert.Contains(";8.50;90", text);
            Assert.Contains(";-", text);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Export_WhenDestinationPathIsEmpty_ShouldThrow(string path)
    {
        Assert.Throws<ArgumentException>(() => _exporter.Export(Array.Empty<FinalGradeReportDto>(), path));
    }
}
