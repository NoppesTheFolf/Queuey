using Noppes.Queuey.Api.Authentication;
using Noppes.Queuey.Core;
using Noppes.Queuey.MongoDB;

namespace Noppes.Queuey.Api;

public class MongoDBConfiguration
{
    public string ConnectionString { get; set; } = null!;

    public string Database { get; set; } = null!;

    public string HistoricDatabase { get; set; } = null!;
}

public class Program
{
    public static void Main(string[] args)
    {
        // Get Queuey's database configuration
        var configurationRoot = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, false)
            .AddUserSecrets<Program>()
            .Build();

        var mongoConf = configurationRoot.GetSection("MongoDB").Get<MongoDBConfiguration>();
        if (mongoConf == null)
            throw new InvalidOperationException("MongoDB not configured");

        var apiKeys = configurationRoot.GetSection("ApiKeys").Get<ICollection<string>>();
        if (apiKeys == null)
            throw new InvalidOperationException("No API keys configured");

        // ASP.NET Core stuff
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;

        // Authentication
        services.AddSingleton(new ApiKeyChecker(apiKeys));

        // Add services to the container.
        services.RegisterCore();
        services.RegisterMongoDB(mongoConf.ConnectionString, mongoConf.Database, mongoConf.HistoricDatabase);

        services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseApiKeyAuthentication();

        app.MapControllers();

        app.Run();
    }
}
