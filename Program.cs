using DormAPI.Database;
using DormAPI.Database.Ado.Common;
using DormAPI.Database.Ado.Repositories;
using DormAPI.Database.EF.Contexts;
using DormAPI.Database.EF.Repositories;
using DormAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);



// 1. Read connection string and register DbContext with DI container 
builder.Services.AddDbContext<DormContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging()
);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

// Add clsExternalLayer to DI container
builder.Services.AddScoped<clsExternalLayer>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<ChatBotRepository>();

// 3. Register the Typed HttpClient for Ollama
var ollamaBaseUrl = builder.Configuration["OllamaConfig:BaseUrl"];
builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.BaseAddress = new Uri(ollamaBaseUrl!);
    client.Timeout = TimeSpan.FromMinutes(3); // Give the local AI time to generate
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAntiforgery();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
