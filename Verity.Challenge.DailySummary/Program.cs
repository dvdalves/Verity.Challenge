
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Verity.Challenge.DailySummary.Infrastructure.Configurations;
using Verity.Challenge.DailySummary.Infrastructure.Persistence;

namespace Verity.Challenge.DailySummary;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //EF Core (PostgreSQL)
        builder.Services.AddDbContext<DailySummaryDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        //Mediatr
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        //AutoMapper
        builder.Services.AddAutoMapper(typeof(DailySummaryProfile));

        //RabbitMQ
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
                var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
                var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";
                cfg.Host(rabbitHost, h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

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
