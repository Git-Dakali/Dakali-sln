using Dakali.Domine.Connection;
using Dakali.Interface.Connection;
using DK.Process;
using DK.Repositories;
using DK.Validator;
using DK.WebApi;
using DK.WebApi.ConvertAutoMapper;
using DK.WebApi.Middleware;

const string DakaliSpecificOrigins = "DakaliAllowedOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new StringToDateTimeConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerDocument(settings =>
{
    settings.Title = "Dakali Web API";
});

#region Cors

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: DakaliSpecificOrigins, policy =>
    {
        policy.AllowCredentials()
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:5173");
    });
});
#endregion

builder.Services.AddScoped<IConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<Dakali.Interface.Connection.ISession, Session>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
DependencyInjectionRepository.Configure(builder.Services);
DependencyInjectionProcess.Configure(builder.Services);
DependencyInjectionValidator.Configure(builder.Services);


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
app.UseCors(DakaliSpecificOrigins);
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();
