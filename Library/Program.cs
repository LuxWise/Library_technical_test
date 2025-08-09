using Library.Data;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<LibraryDbContext>(opt =>
    opt.UseMySql(cs, serverVersion));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbCtx  = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

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
