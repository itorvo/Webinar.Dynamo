using Microsoft.Extensions.Configuration;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Repository;

namespace Webinar.Dynamo.Domain.Repository
{
    public class CountryRepository : DynamoRepository<Country>, ICountryRepository
    {
        public CountryRepository(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
