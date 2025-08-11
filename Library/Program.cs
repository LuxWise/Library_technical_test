using System.Text;
using System.Text.Json;
using Library.Data;
using Library.DTO.Error;
using Library.Middleware;
using Library.Options;
using Library.Seed;
using Library.Services.Auth;
using Library.Services.Auth.Users;
using Library.Services.Books;
using Library.Services.Category;
using Library.Services.Loan;
using Library.Services.Metric;
using Library.Services.Suggestions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Metrics;
using Metrics.Data;

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

builder.Services.AddDbContext<MetricsDbContext>(opt =>
    opt.UseMySql(cs, serverVersion));

//  Jwt configuration settings
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

//  Add services settings
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

//  Services configuration
builder.Services.AddScoped<IAuthService, AuthServices>();
builder.Services.AddScoped<IRegisterServices, RegisterServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBookServices, BookServices>();
builder.Services.AddScoped<ILoanServices, LoanServices>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISuggestionsService, SuggestionsService>();
builder.Services.AddScoped<IMetricServices, MetricServices>();

//  Middleware configuration
builder.Services.AddTransient<ExceptionMiddleware>();


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
        
        
        o.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = 401;
                ctx.Response.ContentType = "application/json";

                var payload = new ErrorResponse()
                {
                    TraceId = ctx.HttpContext.TraceIdentifier,
                    Status = 401,
                    Code = "unauthorized",
                    Title = "Unauthorized",
                    Detail = "Authentication token is missing or invalid."
                };
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            },
            OnForbidden = async ctx =>
            {
                ctx.Response.StatusCode = 403;
                ctx.Response.ContentType = "application/json";
                var payload = new ErrorResponse
                {
                    TraceId = ctx.HttpContext.TraceIdentifier,
                    Status = 403,
                    Code = "forbidden",
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this resource."
                };
                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        };
    });

builder.Services.AddAuthorization();

// Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Library API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce: Bearer {tu_token_jwt}"
    });

    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionMiddleware>();
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
    
    DataSeeder.SeedCategories(dbCtx);
    DataSeeder.SeedBooks(dbCtx);
}

app.Run();
