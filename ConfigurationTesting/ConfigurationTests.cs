using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Logfmt;
using Serilog.Context;
using Xunit.Abstractions;
using Newtonsoft.Json.Linq;

namespace ConfigurationTesting;

public class ConfigurationTests : IDisposable
{

    private readonly ITestOutputHelper output;
    private readonly string basefolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
    public ConfigurationTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    [Fact]
    public void PreservedCaseAndLogLevels()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\preserved_case_and_log_levels.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        LogContext.PushProperty("InterestingProperty", "An interesting property");

        logger.Warning("this is a test!");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_preserved_case_and_log_levels.txt")[0];

        Assert.Contains("level=Warning", lineOfLog);
        Assert.Contains(@"InterestingProperty=""An interesting property""", lineOfLog);
    }

    [Fact]
    public void ComplexSeparators()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\complex_separators.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        LogContext.PushProperty("complex", new {A = "alpha", B = "beta"}, true);

        logger.Error("this is a test!");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_complex.txt")[0];

        Assert.Contains("complex:a=alpha", lineOfLog);
        Assert.Contains("complex:b=beta", lineOfLog);
    }

    [Fact]
    public void PreserveDoubles()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\preserve_double_quotes.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        logger.Error(@"this is a ""double quotes"" test");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_preserve_doubles.txt")[0];

        Assert.Contains(@"msg=""this is a ""double quotes"" test""", lineOfLog);
    }

    [Fact]
    public void RemoveDoubles()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\remove_double_quotes.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        logger.Error(@"this is a ""double quotes"" test");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_remove_doubles.txt")[0];

        Assert.Contains(@"msg=""this is a double quotes test""", lineOfLog);
    }

    [Fact]
    public void ConvertDoubles()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\convert_double_quotes.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        logger.Error(@"this is a ""double quotes"" test");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_convert_doubles.txt")[0];

        Assert.Contains(@"msg=""this is a 'double quotes' test""", lineOfLog);
    }

    [Fact]
    public void EscapeDoubles()
    {
        JsonConfigurationSource jsonConfig = new JsonConfigurationSource(); 
        jsonConfig.Path = "\\resources\\escape_double_quotes.json";
        jsonConfig.FileProvider = new PhysicalFileProvider(basefolder);
        var configuration = new ConfigurationBuilder()
        .Add(jsonConfig)
        .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        logger.Error(@"this is a ""double quotes"" test");
        logger.Dispose();

        var lineOfLog = File.ReadAllLines("..\\..\\..\\resources\\tmp\\testlog_escape_doubles.txt")[0];

        Assert.Contains(@"msg=""this is a \""double quotes\"" test""", lineOfLog);
    }

    public void Dispose()
    {
        Directory.Delete(basefolder + "\\resources\\tmp", true);
    }
}