namespace Pos;

using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pos.Application.Facade;
using Pos.Application.Ports.In;
using Pos.Application.Ports.Out;
using Pos.Domain.Factories;
using Pos.Domain.Model;
using Pos.Domain.Ports;
using Pos.Domain.Services;
using Pos.Domain.Services.Strategies;
using Pos.Infrastructure.Export;
using Pos.Infrastructure.Persistence;
using Pos.Presentation;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        var services = new ServiceCollection();
        ConfigureServices(services);
        using var serviceProvider = services.BuildServiceProvider();
        InitializeDefaultRubric(serviceProvider);
        var menu = serviceProvider.GetRequiredService<ConsoleMenu>();
        menu.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<IGradeCalculationStrategy, WeightedGradeStrategy>();
        services.AddSingleton<FinalGradeCalculator>();
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
        services.AddSingleton<IGradeExporter, CsvGradeExporter>();
        services.AddSingleton<IGradeSystemFacade, GradeSystemFacade>();
        services.AddSingleton<ConsolePresenter>();
        services.AddSingleton<ConsoleMenu>();
    }

    private static void InitializeDefaultRubric(IServiceProvider serviceProvider)
    {
        var rubricRepo = serviceProvider.GetRequiredService<IRubricRepository>();
        var defaultRubric = new Rubric(Guid.NewGuid(), "КПО: Модуль оценивания");
        rubricRepo.Add(defaultRubric);
    }
}