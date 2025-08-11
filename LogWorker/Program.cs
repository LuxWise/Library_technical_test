using LogWorker.LogsBackground;
using LogWorker.Services;
using Metrics.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;            

var builder = Host.CreateApplicationBuilder(args);

// Logging configuration
builder.Services.Configure<LogOptions>(builder.Configuration.GetSection("Logs"));

// Database connection settings
string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "database";
string portStr = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
string db   = Environment.GetEnvironmentVariable("DB_NAME") ?? "library";
string user = Environment.GetEnvironmentVariable("DB_USER") ?? "library_admin";
string pass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "library_password";
if (!uint.TryParse(portStr, out var port)) port = 3306;

var csb = new MySqlConnectionStringBuilder {
    Server = host, Port = port, Database = db, UserID = user, Password = pass,
    TreatTinyAsBoolean = false
};
var cs = csb.ConnectionString;

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
builder.Services.AddDbContext<MetricsDbContext>(opt =>
    opt.UseMySql(cs, serverVersion));

// Add services configuration
builder.Services.AddHostedService<LogIngestionBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbCtx  = scope.ServiceProvider.GetRequiredService<MetricsDbContext>();

    for (int i = 1; i <= 5; i++)
    {
        try
        {
            dbCtx.Database.Migrate();
            logger.LogInformation("Migration apply.");
            break;
        }
        catch (MySqlException ex) when (i < 5)
        {
            logger.LogWarning(ex, "DB it is not ready {Try}/5 in 2s...", i);
            await Task.Delay(2000);
        }
    }
    
    
}

app.Run();