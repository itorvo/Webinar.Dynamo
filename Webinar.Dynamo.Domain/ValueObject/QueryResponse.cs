using System.Collections.Generic;
using Webinar.Dynamo.Repository.ValueObject;

namespace Webinar.Dynamo.Domain.ValueObject
{
    public class QueryResponse<T> where T : class
    {
        public List<T> Elements { get; set; }
        public List<string> PaginationTokens { get; set; }
        public string PaginationToken { get; set; }
        public int Total { get; set; }

        public static implicit operator QueryResponse<T>(FilterResponse<T> response)
        {
            return new QueryResponse<T>
            {
                Elements = response.Elements,
                PaginationToken = response.PaginationToken,
                PaginationTokens = response.PaginationTokens,
                Total = response.Total
            };
        }
    }
}
