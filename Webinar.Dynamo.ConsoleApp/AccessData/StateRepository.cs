using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Webinar.Dynamo.ConsoleApp.Entities;
using Webinar.Dynamo.Repository;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.Filters;

namespace Webinar.Dynamo.ConsoleApp.AccessData
{
    public class StateRepository : DynamoRepository<State>, IStateRepository
    {
        public StateRepository(IConfiguration configuration) : base(configuration)
        {

        }

        private FilterQuery GetFilter(string country, DynamoDbTypeCondition type)
        {
            return new FilterQuery
            {
                AtributeName = "Country",
                Operator = (int)DynamoDbQueryOperator.Equal,
                ValueAtribute = country,
                TypeCondition = type
            };
        }

        public List<State> GetStateByCountryQuery(string country)
        {
            return GetElements(country, DynamoDbTypeCondition.Query);
        }

        public List<State> GetStateByCountryScan(string country)
        {
            return GetElements(country, DynamoDbTypeCondition.Scan);
        }

        private List<State> GetElements(string country, DynamoDbTypeCondition type)
        {
            FilterQuery filters = GetFilter(country, type);
            return GetAllByFilters(filters);
        }
    }
}
