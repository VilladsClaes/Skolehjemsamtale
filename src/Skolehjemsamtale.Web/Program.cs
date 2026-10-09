using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Application.Services;
using Skolehjemsamtale.Domain.Common;
using Skolehjemsamtale.Infrastructure;
using Skolehjemsamtale.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSkolehjemsamtaleInfrastructure(builder.Configuration);

builder.Services.AddScoped<IClock, SystemClock>();
builder.Services.AddScoped<ElevService>();
builder.Services.AddScoped<ObservationService>();
builder.Services.AddScoped<SamtaleService>();

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Skolehjemsamtale API",
        Version = "v1",
        Description = "API til samtaleværktøjet for skole-hjem-samarbejde. Kontrakterne er versionsstyrede (api/v1) og kan konsumeres af Techtree."
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var feature = context.Features.Get<IExceptionHandlerFeature>();
    var ex = feature?.Error;
    context.Response.ContentType = "application/problem+json";
    var (status, title) = ex switch
    {
        DomainException => (StatusCodes.Status400BadRequest, "Domænefejl"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Ikke fundet"),
        _ => (StatusCodes.Status500InternalServerError, "Serverfejl")
    };
    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = status,
        Title = title,
        Detail = app.Environment.IsDevelopment() ? ex?.Message : "Der opstod en fejl."
    });
}));

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skolehjemsamtale v1"));

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapRazorPages();
app.MapHealthChecks("/health");

// Migrer/seed databasen ved opstart. En eksportfil fra det gamle værktøj kan lægges i repo-roden.
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<SkoleDataSeeder>();
    var seedFil = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "seed", "samtale_vaerktoej_data.json");
    if (!File.Exists(seedFil))
        seedFil = Path.Combine(builder.Environment.ContentRootPath, "seed", "samtale_vaerktoej_data.json");
    await seeder.SeedAsync(seedFil);
}

app.Run();

// Nødvendig for integrationstests (WebApplicationFactory).
public partial class Program { }
