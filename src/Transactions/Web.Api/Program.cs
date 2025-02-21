using Application;
using Infrastructure;
using Infrastructure.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Web.Api;

/// <summary>
/// </summary>
public class Program
{
    /// <summary>
    /// </summary>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddAutoMapper(typeof(TransactionProfile));

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
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
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
