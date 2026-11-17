namespace Pos.Application.Ports.Out;

using Pos.Application.Dto;

public interface IGradeExporter
{
    void Export(IReadOnlyCollection<FinalGradeReportDto> reports, string destinationPath);
}