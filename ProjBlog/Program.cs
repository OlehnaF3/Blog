using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjBlog;
using ProjBlog.DbContext;
using ProjBlog.Repository;
using ProjBlog.Services;
using SQLitePCL;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{

    setupAction.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeApi", Version = "v1" });
    setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"

    });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {

            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});
// Регистрация сервиса
builder.Services.AddScoped<IJwtService, JwtService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
Batteries.Init();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
string? connection = Configuration.Config.GetConnectionString("DefaultConnection");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = Configuration.Config["KeySettings:ISSUER"],
        ValidateAudience = true,
        ValidAudience = Configuration.Config["KeySettings:AUDIENCE"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.Config["KeySettings:Key"])),
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero

    });
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Administrator");
    });

    options.AddPolicy("Moderator", policy =>
    {
        policy.RequireRole("Moderator");
    });

});
builder.Services.AddDbContext<BlogDbContext>(options =>
{
    options.UseSqlite(connection);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
