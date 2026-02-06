using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExpenseApproval.Domain;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpenseApproval.Infrastructure.Composition;

/// <summary>
/// Provides initialization of the database schema for the expense approval system when the application starts.
/// </summary>
/// <remarks>This class is intended for use as a hosted service within an ASP.NET Core application. It ensures
/// that the database schema is created before the application begins processing requests. For production environments,
/// consider using database migrations instead of schema creation to manage changes over time.</remarks>
internal sealed class ExpenseApprovalSchemaInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the ExpenseApprovalSchemaInitializer class using the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies required by the schema initializer.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="serviceProvider"/> is null.</exception>
    public ExpenseApprovalSchemaInitializer(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider, nameof(serviceProvider));

        this._serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Starts the ervice by creating a scope to resolve the ExpenseApprovalDbContext and ensuring that the database schema is created.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation of starting the hosted service.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this._serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ExpenseApprovalDbContext>();

        // For demo: EnsureCreated. For production prefer: await db.Database.MigrateAsync(cancellationToken);
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }

    /// <summary>
    /// Initiates a graceful stop of the operation asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the stop operation.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous stop operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
