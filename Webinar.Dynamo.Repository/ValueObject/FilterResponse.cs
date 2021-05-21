using System.Collections.Generic;

namespace Webinar.Dynamo.Repository.ValueObject
{
    public class FilterResponse<T> where T : class
    {
        public List<T> Elements { get; set; }
        public string PaginationToken { get; set; }
        public List<string> PaginationTokens { get; set; }
        public int Total { get; set; }
    }
}
