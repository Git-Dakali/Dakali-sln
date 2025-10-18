using Dakali.Domine.Connection;
using Dakali.Interface.Connection;
using DK.Repositories;
using DK.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerDocument(settings =>
{
    settings.Title = "Dakali Web API";
});

builder.Services.AddScoped<IConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<Dakali.Interface.Connection.ISession, Session>();

DependencyInjectionRepository.Configure(builder.Services);


var app = builder.Build();

// Middleware de transacción por request
app.UseMiddleware<TransactionPerRequestMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();
