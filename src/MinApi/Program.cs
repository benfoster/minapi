using System.Data;
using System.Text.Json.Serialization;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Program>(
    lifetime: ServiceLifetime.Scoped);

builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    opt.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var connectionString = builder.Configuration.GetConnectionString("TodoDb") ?? "Data Source=todos.db";
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();
await EnsureDb(app.Services, app.Logger);

app.MapTodo();
app.Run();


async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    logger.LogInformation("Ensuring database exists at connection string '{connectionString}'", connectionString);

    using var db = services.CreateScope().ServiceProvider.GetRequiredService<IDbConnection>();

    var sql = @"
        CREATE TABLE IF NOT EXISTS todos (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            completed INTEGER DEFAULT 0 NOT NULL CHECK(completed IN (0, 1)),
            created_on DATETIME NOT NULL,
            completed_on DATETIME NULL
        );";

    await db.ExecuteAsync(sql);
}
