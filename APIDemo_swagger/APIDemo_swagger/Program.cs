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

// ��s��Ʈw
// Scaffold-DbContext "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Todo\Todo.mdf;Integrated Security=True;Connect Timeout=30" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -CoNtext TodoContext
builder.Services.AddControllers();
builder.Services.AddControllers().AddNewtonsoftJson(); // JsonPatch

builder.Services.AddDbContext<TodoContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TodoDatabase"))); // DI�`�J���osql�s�u
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<TodoListService>(); // server�`�J
builder.Services.AddScoped<TodoListAsyncService>(); // server�`�J

builder.Services.AddScoped<ITodoListService, TodoLinqService>(); // IOC server DI�`�J �ثe���Ҥ��ݭn
builder.Services.AddScoped<ITodoListService, TodoAutomapperService>();

builder.Services.AddHttpContextAccessor(); // �`�J �~��bcontroller�H�~���a���http

// Add services to the container.�ѨM�~��`�����Ȱ��D
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

//// cookie���ҳ]�w��
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
//{
//    // ���n�J�ɷ|�۰ʾɨ�o�Ӻ��}
//    option.LoginPath = new PathString("/api/Login/NoLogin");
//    // �L�n�J�v���|��o�Ӻ��}
//    option.AccessDeniedPath = new PathString("/api/Login/NoAcess");
//    // �n�J�v���ɭ�
//    // option.ExpireTimeSpan = TimeSpan.FromSeconds(10);
//});

// JWT����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true, // �o��̭n���n������
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true, // �����̭n���n������
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true, // �ɭ��O�_����(�w�]��true)
        ClockSkew = TimeSpan.Zero, // �N�����w�Įɶ��]��0
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"])) // ���_
    };
});



// ����API���n�L����
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    //options.Filters.Add(new TodoAuthorizationFilter()); // �۩w�q����filter
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

// cookie���ҳ]�w�� �]�w���Ǥ����A��
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); // �R�A�ؿ��}��

app.MapControllers();

app.Run();

