using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.Repository;

namespace Webinar.Dynamo.Domain.Domain
{
    public class CountryDomainService : ICountryDomainService
    {
        private readonly ICountryRepository CountryRepository;

        public CountryDomainService(ICountryRepository countryRepository)
        {
            CountryRepository = countryRepository;
        }

        public bool Add(Country country)
        {
            return CountryRepository.Add(country);
        }

        public List<Country> GetAll()
        {
            return CountryRepository.GetAll();
        }
    }
}
