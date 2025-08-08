using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.WebApi;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(null, allowIntegerValues: false));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=horsesforcourses.db"));

builder.Services.AddScoped<IEFCoachRepository, EFCoachRepository>();
builder.Services.AddScoped<IEFCourseRepository, EFCourseRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums(); // Optional, helps with schemas

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HorsesForCourses API",
        Version = "v1",
        Description = "API voor het beheren van cursussen en coaches"
    });
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// app.UseExceptionHandler(appBuilder =>
// {
//     appBuilder.Run(async context =>
//     {
//         context.Response.StatusCode = 500;
//         context.Response.ContentType = "application/problem+json";

//         var problem = new ProblemDetails
//         {
//             Title = "Er is een fout opgetreden",
//             Status = 500,
//             Detail = "Er ging iets mis tijdens het verwerken van je verzoek."
//         };

//         await context.Response.WriteAsJsonAsync(problem);
//     });
// });

app.MapControllers();

app.Run();
