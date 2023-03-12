using Microsoft.AspNetCore.Mvc.Formatters;
using Northwind.WebApi.Repositories;
using Packt.Shared;
//using static System.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNorthwindContext();

// Add services to the container.

builder.Services.AddControllers(options => {
    WriteLine("Default output formatters: ");
    foreach (IOutputFormatter formatter in options.OutputFormatters){
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if(mediaFormatter is  null){
            WriteLine($" {formatter.GetType().Name}");
        } else {
            WriteLine($" {mediaFormatter.GetType().Name}, {string.Join(", ", mediaFormatter.SupportedMediaTypes)}");
        }
    }
}).AddXmlDataContractSerializerFormatters().AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
