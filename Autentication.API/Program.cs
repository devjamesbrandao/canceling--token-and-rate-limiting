using System.Text;
using Autentication.API.Configuration;
using Autentication.API.Middleware;
using Autentication.Core.DTO;
using Autentication.Core.Interfaces;
using Autentication.Core.Services;
using Autentication.Infrastructure.Persistence.Context;
using Autentication.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var jwtSection = builder.Configuration.GetSection("jwt");
var jwtOptions = new JwtOptions();
jwtSection.Bind(jwtOptions);
builder.Services.Configure<JwtOptions>(jwtSection);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<AutenticationContext>(
    x => x.UseSqlite(builder.Configuration["SqlLiteConnection:SqliteConnectionString"])
    .LogTo(Console.WriteLine, LogLevel.Information)
);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.SecretKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddMemoryCache();

builder.Services.AddTransient<IAccountRepository, AccountRepository>();

builder.Services.AddTransient<TokenManagerMiddleware>();

builder.Services.AddTransient<IJwtHandler, JwtHandler>();

builder.Services.AddTransient<IAccountService, AccountService>();

builder.Services.AddTransient<ITokenManager, TokenManager>();

var app = builder.Build();

DbMigrationHelpers.MigrateAsync(app).Wait();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<TokenManagerMiddleware>();

app.Run();
