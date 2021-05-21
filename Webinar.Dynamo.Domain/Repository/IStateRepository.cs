using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.ValueObject;
using Webinar.Dynamo.Repository;
using Webinar.Dynamo.Repository.ValueObject;

namespace Webinar.Dynamo.Domain.Repository
{
    public interface IStateRepository : IDynamoRepository<State>
    {
        FilterResponse<State> GetStateByCountry(string country, int? limit, string paginationToken);
        List<State> GetStateByName(string nameState);
        List<State> GetStateByNotName(string country, string nameState);
        List<State> GetStateByPoblationLE(int number);
        List<State> GetStateByPoblationLT(int number);
        List<State> GetByGreaterThanOrEqualOperator(int number);
        QueryResponse<State> GetAllPaginated(string paginationToken, int limit);
        List<State> GetByGreaterThanOperator(int number);
        List<State> GetByBeginsWithOperator(string name);
        List<State> GetByIsNotNullOperator();
        List<State> GetByIsNullOperator();
        List<State> GetByContainsOperator();
        List<State> GetByNotContainsOperator();
        List<State> GetByInOperator(List<string> names);
        List<State> GetByBetweenOperator(int initValue, int finalValue);
    }
}
