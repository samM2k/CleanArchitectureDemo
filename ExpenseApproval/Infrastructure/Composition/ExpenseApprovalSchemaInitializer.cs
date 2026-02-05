using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExpenseApproval.Infrastructure.Composition;

internal sealed class ExpenseApprovalSchemaInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ExpenseApprovalSchemaInitializer(IServiceProvider serviceProvider) => this._serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = this._serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // For demo: EnsureCreated. For production prefer: await db.Database.MigrateAsync(cancellationToken);
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
