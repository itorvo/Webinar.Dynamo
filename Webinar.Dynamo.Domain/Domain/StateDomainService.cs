using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.Repository;
using Webinar.Dynamo.Domain.ValueObject;

namespace Webinar.Dynamo.Domain.Domain
{
    public class StateDomainService : IStateDomainService
    {
        private readonly IStateRepository StateRepository;

        public StateDomainService(IStateRepository stateRepository)
        {
            StateRepository = stateRepository;
        }

        public List<State> GetAll()
        {
            return StateRepository.GetAll();
        }

        public QueryResponse<State> GetAll(string paginationToken, int limit)
        {
            return StateRepository.GetAllPaginated(paginationToken, limit);
        }

        public bool Add(State state)
        {
            return !ExistsState(state) && StateRepository.Add(state);
        }
        
        public bool Remove(string country, string code)
        {
            var state = new State { Country = country, Code = code };
            return ExistsState(state) && StateRepository.Remove(state);
        }

        public bool Update(State state)
        {
            return ExistsState(state) && StateRepository.Update(state);
        }

        private bool ExistsState(State state)
        {
            return StateRepository.Get(state) != null;
        }
    }
}
