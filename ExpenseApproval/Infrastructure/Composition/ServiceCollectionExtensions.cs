// <copyright file="ServiceCollectionExtensions.cs" company="CleanArchitectureDemoCompany">
// Copyright (c) CleanArchitectureDemoCompany. All rights reserved.
// </copyright>

namespace ExpenseApproval.Infrastructure.Composition;

using ExpenseApproval.Application;
using ExpenseApproval.Domain;
using ExpenseApproval.Infrastructure.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the ExpenseApproval module's application and infrastructure services with
/// an ASP.NET Core dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the ExpenseApproval module (Application + Infrastructure services).
    /// </summary>
    /// <remarks>
    /// Does not register controllers (presentation concerns) - the Host should do that.
    /// </remarks>
    /// <param name="services">The service collection to add the ExpenseApproval services to.</param>
    /// <param name="configure">Configuration action for ExpenseApprovalOptions, which controls various aspects of the module's setup (e.g. whether to use in-memory SQLite or a real database).</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddExpenseApproval(
        this IServiceCollection services,
        Action<ExpenseApprovalOptions> configure)
    {
        var options = new ExpenseApprovalOptions();
        configure(options);

        // -------------------------
        // EF Core / DbContext
        // -------------------------
        if (options.UseSqliteInMemory)
        {
            // One shared connection that must remain open for the DB lifetime.
            var conn = new SqliteConnection("Data Source=:memory:");
            conn.Open();

            // Turn on FK enforcement
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA foreign_keys = ON;";
                cmd.ExecuteNonQuery();
            }

            services.AddSingleton(conn);

            services.AddDbContext<ExpenseApprovalDbContext>((sp, db) =>
            {
                db.UseSqlite(sp.GetRequiredService<SqliteConnection>());

                if (options.EnableEfSensitiveLogging)
                {
                    db.EnableSensitiveDataLogging();
                }

                db.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
            });
        }
        else
        {
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException("ExpenseApprovalOptions.ConnectionString is required when not using in-memory SQLite.");
            }

            services.AddDbContext<ExpenseApprovalDbContext>(db =>
            {
                db.UseSqlite(options.ConnectionString);

                if (options.EnableEfSensitiveLogging)
                {
                    db.EnableSensitiveDataLogging();
                }
            });
        }

        // -------------------------
        // Application + Domain services
        // -------------------------

        // Policy contract belongs in Domain; implementation can live in Application.
        services.AddScoped<IExpensePolicy, CompanyExpensePolicy>();

        // Repo interface in Application, implementation in Infrastructure
        services.AddScoped<IExpenseRepository, EfExpenseRepository>();

        // Use-case service
        services.AddScoped<ExpenseService>();

        // -------------------------
        // Startup task for schema creation (optional)
        // -------------------------
        if (options.EnsureCreatedOnStartup)
        {
            services.AddHostedService<ExpenseApprovalSchemaInitializer>();
        }

        return services;
    }
}
