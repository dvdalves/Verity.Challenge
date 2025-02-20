using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Verity.Challenge.Transactions.Infrastructure.Configurations;
using Verity.Challenge.Transactions.Infrastructure.Persistence;

namespace Verity.Challenge.Transactions;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //EF Core (PostgreSQL)
        builder.Services.AddDbContext<TransactionsDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        //Meditar
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        //AutoMapper
        builder.Services.AddAutoMapper(typeof(TransactionProfile));

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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
