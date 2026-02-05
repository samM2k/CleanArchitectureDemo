
using ExpenseApproval.Infrastructure.Composition;

namespace Host;

public class Program
{
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
