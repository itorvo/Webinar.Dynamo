using Amazon.DynamoDBv2.DocumentModel;

namespace Webinar.Dynamo.Repository.Enumerators
{
    public enum DynamoDbQueryOperator
    {
        Equal = QueryOperator.Equal,
        LessThanOrEqual = QueryOperator.LessThanOrEqual,
        LessThan = QueryOperator.LessThan,
        GreaterThanOrEqual = QueryOperator.GreaterThanOrEqual,
        GreaterThan = QueryOperator.GreaterThan,
        BeginsWith = QueryOperator.BeginsWith,
        Between = QueryOperator.Between
    }

    public enum DynamoDbBatchOperator
    {
        Put = 1,
        Delete = 2
    }

    public enum DynamoDbScanOperator
    {
        Equal = ScanOperator.Equal,
        NotEqual = ScanOperator.NotEqual,
        LessThanOrEqual = ScanOperator.LessThanOrEqual,
        LessThan = ScanOperator.LessThan,
        GreaterThanOrEqual = ScanOperator.GreaterThanOrEqual,
        GreaterThan = ScanOperator.GreaterThan,
        IsNotNull = ScanOperator.IsNotNull,
        IsNull = ScanOperator.IsNull,
        Contains = ScanOperator.Contains,
        NotContains = ScanOperator.NotContains,
        BeginsWith = ScanOperator.BeginsWith,
        In = ScanOperator.In,
        Between = ScanOperator.BeginsWith
    }

    public enum DynamoDbTypeCondition
    {
        Query = 0,
        Scan = 1
    }
}
