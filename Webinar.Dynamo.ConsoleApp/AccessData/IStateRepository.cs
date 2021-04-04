using System.Collections.Generic;
using Webinar.Dynamo.ConsoleApp.Entities;
using Webinar.Dynamo.Repository;

namespace Webinar.Dynamo.ConsoleApp.AccessData
{
    public interface IStateRepository : IDynamoRepository<State>
    {
        List<State> GetStateByCountryQuery(string country);
        List<State> GetStateByCountryScan(string country);
    }
}
