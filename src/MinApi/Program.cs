using System.Data;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<Program>(
    lifetime: ServiceLifetime.Scoped);

var connectionString = builder.Configuration.GetConnectionString("TodoDb") ?? "Data Source=todos.db";
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));

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
            created_on TEXT NOT NULL,
            completed_on TEXT NULL
        );";
    
    await db.ExecuteAsync(sql);
}