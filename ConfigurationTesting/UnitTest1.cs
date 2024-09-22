using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Logfmt;
using Serilog.Context;
using Xunit.Abstractions;

namespace ConfigurationTesting;

public class UnitTest1 : IDisposable
{

    private readonly ITestOutputHelper output;
    public UnitTest1(ITestOutputHelper output)
    {
        this.output = output;
    }
    [Fact]
    public void PreservedCaseAndLogLevels()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\preserved_case_and_log_levels.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        logger.Warning("this is a test!");

        Assert.False(false);
    }

    [Fact]
    public void ComplexSeparators()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\complex_separators.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        LogContext.PushProperty("complex", new {A = "alpha", B = "beta"}, true);

        logger.Error("this is a test!");

        Assert.False(false);
    }

    public void Dispose()
    {
        foreach (var file in Directory.GetFiles(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\resources\\tmp"))
        {
            Directory.Delete(file);
        }
    }
}