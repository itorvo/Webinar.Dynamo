using Amazon.DynamoDBv2;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.Converters
{
    internal static class FilterOperatorConverter
    {
        internal static ComparisonOperator GetComparisonOperator(this DynamoDbFilterOperator enumOperator)
        {
            return enumOperator switch
            {
                DynamoDbFilterOperator.Equal => ComparisonOperator.EQ,
                DynamoDbFilterOperator.NotEqual => ComparisonOperator.NE,
                DynamoDbFilterOperator.LessThanOrEqual => ComparisonOperator.LE,
                DynamoDbFilterOperator.LessThan => ComparisonOperator.LT,
                DynamoDbFilterOperator.GreaterThanOrEqual => ComparisonOperator.GE,
                DynamoDbFilterOperator.GreaterThan => ComparisonOperator.GT,
                DynamoDbFilterOperator.BeginsWith => ComparisonOperator.BEGINS_WITH,
                DynamoDbFilterOperator.Between => ComparisonOperator.BETWEEN,
                DynamoDbFilterOperator.IsNotNull => ComparisonOperator.NOT_NULL,
                DynamoDbFilterOperator.IsNull => ComparisonOperator.NULL,
                DynamoDbFilterOperator.Contains => ComparisonOperator.CONTAINS,
                DynamoDbFilterOperator.NotContains => ComparisonOperator.NOT_CONTAINS,
                DynamoDbFilterOperator.In => ComparisonOperator.IN,
                _ => ComparisonOperator.EQ,
            };
        }

    }
}
