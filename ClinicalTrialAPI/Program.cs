using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClinicalTrialAPI.Data;
using System.Security.Cryptography.X509Certificates;
using ClinicalTrialAPI.Models;
var builder = WebApplication.CreateBuilder(args);

var server = Environment.GetEnvironmentVariable("DBServer") ?? "mssql";
var port = Environment.GetEnvironmentVariable("DBPort") ?? "1433";
var user = Environment.GetEnvironmentVariable("DBUser") ?? "SA";
var password = Environment.GetEnvironmentVariable("DBPassword") ?? "StronKPassW0rd1!";

builder.Services.AddDbContext<ClinicalTrialAPIContext>(options =>
    options.UseSqlServer($"Server={server},{port};User ID={user};Password={password};Encrypt=False"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

PrepDB.PrepPopulation(app);
// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
