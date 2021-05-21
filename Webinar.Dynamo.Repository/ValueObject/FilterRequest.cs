using System;
using System.Collections.Generic;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.ValueObject
{
    public class FilterRequest : ICloneable
    {
        public List<string> AttributesToGet { get; set; }
        public bool BackwardSearch { get; set; }
        public List<FilterCondition> Conditions { get; set; }
        public int? CurrentPage { get; set; }
        internal bool GetTotal { get; set; }
        public string IndexName { get; set; }
        public int Limit { get; set; }
        public int? NewLimit { get; set; }
        public OperatorConditional Operator { get; set; }
        public string PaginationToken { get; set; }

        public FilterRequest()
        {
            Operator = OperatorConditional.And;
            Conditions = new List<FilterCondition>();
            PaginationToken = "{}";
        }

        public object Clone()
        {
            return new FilterRequest
            {
                AttributesToGet = AttributesToGet,
                BackwardSearch = BackwardSearch,
                Conditions = Conditions,
                CurrentPage = CurrentPage,
                GetTotal = false,
                IndexName = IndexName,
                Limit = Limit,
                NewLimit = NewLimit,
                Operator = Operator,
                PaginationToken = PaginationToken
            };
        }
    }
}
