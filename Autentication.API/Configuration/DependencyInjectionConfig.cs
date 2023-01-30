namespace Autentication.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<AutenticationContext>(
                x => x.UseSqlite(builder.Configuration["SqlLiteConnection:SqliteConnectionString"])
                .LogTo(Console.WriteLine, LogLevel.Information)
            );

            services.AddMemoryCache();

            services.AddTransient<IAccountRepository, AccountRepository>();

            services.AddTransient<TokenManagerMiddleware>();

            services.AddTransient<IJwtHandler, JwtHandler>();

            services.AddTransient<IAccountService, AccountService>();

            services.AddTransient<ITokenManager, TokenManager>();

            return services;
        }
    }
}