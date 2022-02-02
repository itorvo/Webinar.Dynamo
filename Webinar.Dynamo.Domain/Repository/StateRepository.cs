using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.ValueObject;
using Webinar.Dynamo.Repository;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.ValueObject;

namespace Webinar.Dynamo.Domain.Repository
{
    public class StateRepository : DynamoRepository<State>, IStateRepository
    {
        private readonly FilterCondition FilterCountryCO;

        public StateRepository(IConfiguration configuration) : base(configuration)
        {
            FilterCountryCO = new FilterCondition
            {
                AtributeName = nameof(State.Country),
                Operator = DynamoDbFilterOperator.Equal,
                ValueAtribute = "CO",
                TypeCondition = DynamoDbTypeCondition.Query
            };

            IndexNames = new List<string> { "cfckmlkc", "dscdcs" };
        }

        public QueryResponse<State> GetAllPaginated(string paginationToken, int limit)
        {
            FilterRequest request = new FilterRequest
            {
                Limit = limit,
                PaginationToken = paginationToken
            };

            return GetAllByFilters(request);
        }

        public List<State> GetByBeginsWithOperator(string name)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        Operator = DynamoDbFilterOperator.BeginsWith,
                        ValueAtribute = name,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };

            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByBetweenOperator(int initValue, int finalValue)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.Between,
                        ValueAtribute = new List<int>{ initValue, finalValue },
                        TypeCondition = DynamoDbTypeCondition.Query
                    },
                }
            };

            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByContainsOperator()
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        ValueAtribute = "cha",
                        Operator = DynamoDbFilterOperator.Contains,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };

            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByGreaterThanOperator(int number)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.GreaterThan,
                        ValueAtribute = number,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };

            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByGreaterThanOrEqualOperator(int number)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.GreaterThanOrEqual,
                        ValueAtribute = number,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByInOperator(List<string> names)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        Operator = DynamoDbFilterOperator.In,
                        ValueAtribute = names,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByIsNotNullOperator()
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.IsNotNull,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByIsNullOperator()
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "Position",
                        Operator = DynamoDbFilterOperator.IsNull,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetByNotContainsOperator()
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        ValueAtribute = "cha",
                        Operator = DynamoDbFilterOperator.NotContains,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public FilterResponse<State> GetStateByCountry(string country, int? limit, string paginationToken)
        {
            List<FilterCondition> filters = country != null ? new List<FilterCondition>
                {
                    new FilterCondition
                    {
                        AtributeName = nameof(State.Country),
                        Operator = DynamoDbFilterOperator.Equal,
                        ValueAtribute = country,
                        TypeCondition = DynamoDbTypeCondition.Query
                    }
                } : new List<FilterCondition>();

            FilterRequest request = new FilterRequest
            {
                Conditions = filters,
                PaginationToken = paginationToken,
                Limit = limit ?? 0
            };

            return GetAllByFilters(request);
        }

        public List<State> GetStateByName(string nameState)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>
                {
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        Operator = DynamoDbFilterOperator.Equal,
                        ValueAtribute = nameState,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    }
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetStateByNotName(string country, string nameState)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    new FilterCondition
                    {
                        AtributeName = "Country",
                        Operator = DynamoDbFilterOperator.Equal,
                        ValueAtribute = country,
                        TypeCondition = DynamoDbTypeCondition.Query
                    },
                    new FilterCondition
                    {
                        AtributeName = "Name",
                        Operator = DynamoDbFilterOperator.NotEqual,
                        ValueAtribute = nameState,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    }
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetStateByPoblationLE(int number)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.LessThanOrEqual,
                        ValueAtribute = number,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }

        public List<State> GetStateByPoblationLT(int number)
        {
            FilterRequest request = new FilterRequest
            {
                Conditions = new List<FilterCondition>{
                    FilterCountryCO,
                    new FilterCondition
                    {
                        AtributeName = "NumberCitizens",
                        Operator = DynamoDbFilterOperator.LessThan,
                        ValueAtribute = number,
                        TypeCondition = DynamoDbTypeCondition.Scan
                    },
                }
            };
            return GetAllByFilters(request).Elements;
        }
    }
}
