
using ExpenseApproval.Infrastructure.Composition;

namespace Host;

/// <summary>
/// Provides the entry point for the CleanArchitectureDemo application.
/// </summary>
public class Program
{
    /// <summary>
    /// Initializes and runs the ASP.NET Core web application.
    /// </summary>
    /// <param name="args">An array of command-line arguments provided to the application at startup.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(ExpenseApproval.Api.Controllers.ExpensesController).Assembly);

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register module services (Application + Infrastructure)
        SQLitePCL.Batteries.Init();
        builder.Services.AddExpenseApproval(opt =>
        {
            opt.UseSqliteInMemory = true;                  // demo mode
            opt.EnsureCreatedOnStartup = true;             // auto create tables
            opt.EnableEfSensitiveLogging = builder.Environment.IsDevelopment();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
