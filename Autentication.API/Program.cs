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

builder.Services.ResolveDependencies();

builder.Services.AddSwaggerConfig();

builder.Services.AddAutenticationJWTConfig();

var app = builder.Build();

DbMigrationHelpers.MigrateAsync(app).Wait();

app.UseSwaggerConfig();

app.MapControllers();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAutenticationJWT();

app.UseMiddleware<TokenManagerMiddleware>();

app.Run();
