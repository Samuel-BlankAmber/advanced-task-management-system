using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Commands;
using TaskManagement.Api.Data;
using TaskManagement.Api.Queries;
using TaskManagement.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

string? dbConnectionString = builder.Configuration.GetConnectionString("TasksDb");
builder.Services.AddDbContext<TasksDb>(o => o.UseNpgsql(dbConnectionString));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ITaskCommandHandler, TaskCommandHandler>();
builder.Services.AddScoped<ITaskQueryHandler, TaskQueryHandler>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TasksDb>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
