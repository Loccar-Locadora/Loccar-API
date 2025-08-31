using LoccarApplication;
using LoccarApplication.Interfaces;
using LoccarInfra.ORM.model;
using LoccarInfra.Repositories;
using LoccarInfra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ILocatarioApplication, LocatarioApplication>();
builder.Services.AddScoped<ILocatarioRepository, LocatarioRepository>();

string connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<DataBaseContext>(options => options.UseNpgsql(connectionString));

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
