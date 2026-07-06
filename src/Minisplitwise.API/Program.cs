using Minisplitwise.Application;
using Minisplitwise.Infrastructure;
using Minisplitwise.API.Endpoints;
using Minisplitwise.API.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Minisplitwise API",
        Version = "v1",
        Description = "REST API for managing shared expenses. Register members, create groups, and add participants to groups — the foundation for splitting bills among friends."
    });
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapMemberEndpoints();
app.MapGroupEndpoints();

app.Run();
