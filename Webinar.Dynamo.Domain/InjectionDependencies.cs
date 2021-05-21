using Microsoft.Extensions.DependencyInjection;
using Webinar.Dynamo.Domain.Domain;
using Webinar.Dynamo.Domain.Repository;

namespace Webinar.Dynamo.Domain
{
    public static class InjectionDependencies
    {
        public static void InjectionServices(IServiceCollection services)
        {
            services.AddScoped<IStateDomainService, StateDomainService>();
            services.AddScoped<ICountryDomainService, CountryDomainService>();

            services.AddScoped<IStateRepository, StateRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
        }
    }
}
