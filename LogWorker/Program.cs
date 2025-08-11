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
app.Run();