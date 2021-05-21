using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.ValueObject;

namespace Webinar.Dynamo.Domain.Domain
{
    public interface IStateDomainService
    {
        bool Add(State state);
        List<State> GetAll();
        QueryResponse<State> GetAll(string paginationToken, int limit);
        bool Remove(string country, string code);
        bool Update(State state);
    }
}