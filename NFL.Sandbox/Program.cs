using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NFL.Database.Context;
using System;

namespace NFL.SandBox
{
    class Program
    {
        public static void LoadConfiguration(HostBuilderContext host, IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services
                .AddDbContext<NflContext>(options =>
                {
                    options.UseSqlServer(
                        host.Configuration.GetConnectionString("ConnectionString"), builder =>
                            builder.MigrationsAssembly("migration.presentence"));
                }, ServiceLifetime.Singleton)
                .AddHostedService<Startup>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(LoadConfiguration)
                .ConfigureServices(ConfigureServices);

        private static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            Console.WriteLine("Hello World!");
        }
    }

    internal class Startup : BackgroundService
    {
        private readonly NflContext context;

        public Startup(NflContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}