using APIDemo_swagger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 更新資料庫
// Scaffold-DbContext "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Todo\Todo.mdf;Integrated Security=True;Connect Timeout=30" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -CoNtext TodoContext
builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDatabase")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//// Add services to the container.解決外鍵循環取值問題
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//});

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
