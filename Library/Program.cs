using System.Text;
using Library.Data;
using Library.Options;
using Library.Services.Auth;
using Library.Services.Books;
using Library.Services.Category;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

//  Database connection settings
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

//  Jwt configuration settings
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

//  Add services settings
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Auth configuration settings
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IRegisterServices, RegisterServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookServices, BookServices>();

// JWT Bearer
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
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
