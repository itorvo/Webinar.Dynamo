using System;
using System.Collections.Generic;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.ValueObject
{
    [Obsolete("Should use FilterCondition")]
    public class FilterQuery
    {
        public string AtributeName { get; set; }
        public int Operator { get; set; }
        public DynamoDbTypeCondition TypeCondition { get; set; }
        public object ValueAtribute { get; set; }
        public object ValueAtributeFinal { get; set; }

        public static explicit operator FilterCondition(FilterQuery filter)
        {
            DynamoDbFilterOperator @operator = (DynamoDbFilterOperator)filter.Operator;

            FilterCondition condition = new FilterCondition
            {
                AtributeName = filter.AtributeName,
                Operator = @operator,
                TypeCondition = filter.TypeCondition,
                ValueAtribute = @operator == DynamoDbFilterOperator.Between ? new List<object> { filter.ValueAtribute, filter.ValueAtributeFinal } : filter.ValueAtribute
            };

            return condition;
        }
    }
}
