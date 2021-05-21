using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Webinar.Dynamo.Domain;

namespace Webinar.Dynamo.LambdaState
{
    public class Startup
    {
        public readonly IConfigurationRoot Configuration;
        public readonly ServiceProvider ServiceProvider;

        public Startup()
        {
            Configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", true, true).Build();

            IServiceCollection services = new ServiceCollection();

            InjectionDependencies.InjectionServices(services);

            services.AddSingleton<IConfiguration>(Configuration);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
