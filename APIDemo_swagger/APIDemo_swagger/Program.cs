using APIDemo_swagger.Filters;
using APIDemo_swagger.Interfaces;
using APIDemo_swagger.Models;
using APIDemo_swagger.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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
builder.Services.AddControllers().AddNewtonsoftJson(); // JsonPatch

builder.Services.AddDbContext<TodoContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDatabase"))); // DI注入取得sql連線
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<TodoListService>(); // server注入
builder.Services.AddScoped<TodoListAsyncService>(); // server注入

builder.Services.AddScoped<ITodoListService, TodoLinqService>(); // IOC server DI注入 目前情境不需要
builder.Services.AddScoped<ITodoListService, TodoAutomapperService>();

builder.Services.AddHttpContextAccessor(); // 注入 才能在controller以外的地方取http

// Add services to the container.解決外鍵循環取值問題
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

//// cookie驗證設定檔
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    // 未登入時會自動導到這個網址
//    option.LoginPath = new PathString("/api/Login/NoLogin");
//    // 無登入權限會到這個網址
//    option.AccessDeniedPath = new PathString("/api/Login/NoAcess");
//    // 登入權限時限
//    // option.ExpireTimeSpan = TimeSpan.FromSeconds(10);
//});

// JWT驗證
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, // 發行者要不要受驗證
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true, // 接收者要不要受驗證
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true, // 時限是否驗證(預設為true)
        ClockSkew = TimeSpan.Zero, // 將期限緩衝時間設為0
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"])) // 金鑰
    };
});



// 全部API都要過驗證
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    //options.Filters.Add(new TodoAuthorizationFilter()); // 自定義驗證filter
    options.Filters.Add(typeof(TodoActionFilter));
    options.Filters.Add(typeof(TodoResultFilter));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// cookie驗證設定檔 設定順序不能顛倒
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); // 靜態目錄開關

app.MapControllers();

app.Run();

