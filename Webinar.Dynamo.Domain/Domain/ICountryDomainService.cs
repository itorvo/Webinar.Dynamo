using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;

namespace Webinar.Dynamo.Domain.Domain
{
    public interface ICountryDomainService
    {
        bool Add(Country country);
        List<Country> GetAll();
    }
}