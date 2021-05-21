using Amazon.DynamoDBv2.DocumentModel;
using System;

namespace Webinar.Dynamo.Repository.Enumerators
{
    [Obsolete("Should use DynamoDbFilterOperator")]
    public enum DynamoDbQueryOperator
    {
        Equal = DynamoDbFilterOperator.Equal,
        LessThanOrEqual = DynamoDbFilterOperator.LessThanOrEqual,
        LessThan = DynamoDbFilterOperator.LessThan,
        GreaterThanOrEqual = DynamoDbFilterOperator.GreaterThanOrEqual,
        GreaterThan = DynamoDbFilterOperator.GreaterThan,
        BeginsWith = DynamoDbFilterOperator.BeginsWith,
        Between = DynamoDbFilterOperator.Between
    }

    [Obsolete("Should use DynamoDbFilterOperator")]
    public enum DynamoDbScanOperator
    {
        Equal = DynamoDbFilterOperator.Equal,
        NotEqual = DynamoDbFilterOperator.NotEqual,
        LessThanOrEqual = DynamoDbFilterOperator.LessThanOrEqual,
        LessThan = DynamoDbFilterOperator.LessThan,
        GreaterThanOrEqual = DynamoDbFilterOperator.GreaterThanOrEqual,
        GreaterThan = DynamoDbFilterOperator.GreaterThan,
        IsNotNull = DynamoDbFilterOperator.IsNotNull,
        IsNull = DynamoDbFilterOperator.IsNull,
        Contains = DynamoDbFilterOperator.Contains,
        NotContains = DynamoDbFilterOperator.NotContains,
        BeginsWith = DynamoDbFilterOperator.BeginsWith,
        In = DynamoDbFilterOperator.In,
        Between = DynamoDbFilterOperator.Between
    }

    public enum DynamoDbFilterOperator
    {
        Equal = 1,
        NotEqual = 2,
        LessThanOrEqual = 3,
        LessThan = 4,
        GreaterThanOrEqual = 5,
        GreaterThan = 6,
        BeginsWith = 7,
        Between = 8,
        IsNotNull = 9,
        IsNull = 10,
        Contains = 11,
        NotContains = 12,
        In = 13
    }

    internal enum EnumKeyType
    {
        Hash = 1,
        Range = 2
    }

    public enum DynamoDbBatchOperator
    {
        Put = 1,
        Delete = 2
    }

    public enum DynamoDbTypeCondition
    {
        Query = 1,
        Scan = 2
    }

    public enum OperatorConditional
    {
        And = ConditionalOperatorValues.And,
        Or = ConditionalOperatorValues.Or
    }
}
