using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skolehjemsamtale.Application.Abstractions;
using Skolehjemsamtale.Infrastructure.Integration;
using Skolehjemsamtale.Infrastructure.Persistence;
using Skolehjemsamtale.Infrastructure.Persistence.Repositories;
using Skolehjemsamtale.Infrastructure.Seed;

namespace Skolehjemsamtale.Infrastructure;

/// <summary>
/// Samler registreringen af persistence og integration, så værtsapplikationen (Web)
/// kun behøver at kalde AddSkolehjemsamtaleInfrastructure.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSkolehjemsamtaleInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=skolehjemsamtale.db";

        services.AddDbContext<SkoleDbContext>(opt => opt.UseSqlite(connectionString, sqlite =>
            // Aggregater hentes med flere kollektioner; split query undgår kartesisk eksplosion.
            sqlite.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<IElevRepository, ElevRepository>();
        services.AddScoped<IObservationRepository, ObservationRepository>();
        services.AddScoped<ISamtaleRepository, SamtaleRepository>();
        services.AddScoped<IHjemmeIndsigtRepository, HjemmeIndsigtRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IIntegrationPublisher, EfIntegrationPublisher>();
        services.AddScoped<SkoleDataSeeder>();

        services.Configure<TechtreeOptions>(configuration.GetSection(TechtreeOptions.SectionName));

        var techtreeBaseUrl = configuration[$"{TechtreeOptions.SectionName}:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(techtreeBaseUrl))
        {
            services.AddHttpClient<ITechtreeGateway, HttpTechtreeGateway>(client =>
            {
                client.BaseAddress = new Uri(techtreeBaseUrl!);
                client.Timeout = TimeSpan.FromSeconds(30);
            });
        }
        else
        {
            services.AddSingleton<ITechtreeGateway, NoopTechtreeGateway>();
        }

        services.AddHostedService<OutboxProcessor>();
        return services;
    }
}
