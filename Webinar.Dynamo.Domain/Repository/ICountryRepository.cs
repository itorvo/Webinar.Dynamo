using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Repository;

namespace Webinar.Dynamo.Domain.Repository
{
    public interface ICountryRepository : IDynamoRepository<Country>
    {
    }
}
