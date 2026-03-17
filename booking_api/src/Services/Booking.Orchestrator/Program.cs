using Booking.Orchestrator.Application.Behaviors;
using Booking.Orchestrator.Application.Services;
using Booking.Orchestrator.Infrastructure.Data;
using Booking.Orchestrator.Infrastructure.Kafka;
using Booking.Orchestrator.Infrastructure.Repositories;
using Confluent.Kafka;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Infrastructure.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BookingDb")));

// Kafka Settings
var kafkaSettings = builder.Configuration.GetSection("Kafka").Get<KafkaSettings>()!;
builder.Services.AddSingleton(kafkaSettings);

// Kafka Producer
builder.Services.AddSingleton<IProducer<string, string>>(sp =>
{
    var config = new ProducerConfig { BootstrapServers = kafkaSettings.BootstrapServers };
    return new ProducerBuilder<string, string>(config).Build();
});
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// Kafka Consumer
builder.Services.AddSingleton<IKafkaConsumer, KafkaConsumer>();
builder.Services.AddHostedService<BookingSagaConsumer>();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<Program>());

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Repositories
builder.Services.AddScoped<IBookingSagaRepository, BookingSagaRepository>();

// Services
builder.Services.AddScoped<BookingSagaOrchestrator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
