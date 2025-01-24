using ClinicalTrialAPI.Data;
using ClinicalTrialAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string server = Environment.GetEnvironmentVariable("DBServer") ?? "mssql";
string port = Environment.GetEnvironmentVariable("DBPort") ?? "1433";
string user = Environment.GetEnvironmentVariable("DBUser") ?? "SA";
string password = Environment.GetEnvironmentVariable("DBPassword") ?? "StronKPassW0rd1!";

builder.Services.AddDbContext<ClinicalTrialAPIContext>(options =>
    options.UseSqlServer($"Server={server},{port};User ID={user};Password={password};Encrypt=False"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinical Trial API",
        Version = "v1",
        Description = "API for managing clinical trial metadata"
    });
    c.EnableAnnotations();
});

WebApplication app = builder.Build();

PrepDB.PrepPopulation(app);
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinical Trial API v1");
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
