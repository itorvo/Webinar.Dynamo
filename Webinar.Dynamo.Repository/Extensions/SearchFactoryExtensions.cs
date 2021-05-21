using Amazon.DynamoDBv2;
using System;
using System.Collections;
using System.Linq;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.Factories;

namespace Webinar.Dynamo.Repository.Extensions
{
    internal static class SearchFactoryExtensions
    {
        internal static void ValidDuplicateCondition(this SearchFactory search)
        {
            string fieldErrors;
            var groups = search.Conditions.GroupBy(c => c.AtributeName).Where(g => g.Count() > 1);
            if (groups.Any())
            {
                fieldErrors = string.Join(",", groups.Where(g => g.Count() > 1).Select(g => g.Key));
                ArgumentException ex = new ArgumentException($"The field{(groups.Count() > 1 ? "s" : "")} {fieldErrors} only admits one condition");
                throw ex;
            }
        }

        internal static void ValidBetweenCondition(this SearchFactory search)
        {
            string fieldErrors;
            var conditions = search.Conditions.FindAll(c => c.Operator == DynamoDbFilterOperator.Between && ((IList)c.ValueAtribute).Count != 2);
            if (conditions.Any())
            {
                fieldErrors = string.Join(",", conditions.Select(c => c.AtributeName));
                ArgumentException ex = new ArgumentException($"The condition of the field{(conditions.Count > 1 ? "s" : "")} {fieldErrors} is of type Between and does not have two elements.");
                throw ex;
            }
        }

        internal static void ValidInCondition(this SearchFactory search)
        {
            string fieldErrors;
            var conditions = search.Conditions.FindAll(c => c.Operator == DynamoDbFilterOperator.In && ((IList)c.ValueAtribute).Count == 0);
            if (conditions.Any())
            {
                fieldErrors = string.Join(",", conditions.Select(c => c.AtributeName));
                ArgumentException ex = new ArgumentException($"The condition of the field{(conditions.Count > 1 ? "s" : "")} {fieldErrors} {(conditions.Count > 1 ? "are" : "is")} of type In and must have at least one element.");
                throw ex;
            }
        }

        internal static string ValidIndexAndKeys(this SearchFactory search)
        {
            string hashKey;

            if (string.IsNullOrEmpty(search.IndexName))
            {
                hashKey = search.Table.HashKeys.FirstOrDefault();
            }
            else if (search.Table.GlobalSecondaryIndexes.TryGetValue(search.IndexName, out var indexInfo))
            {
                hashKey = indexInfo.KeySchema.Find(key => key.KeyType == KeyType.HASH).AttributeName;
            }
            else
            {
                ArgumentException ex = new ArgumentException($"The Index {search.IndexName} not exists");
                throw ex;
            }

            int numberFilters = search.Conditions.Count(c => !string.IsNullOrEmpty(hashKey) && c.AtributeName.Equals(hashKey) && c.Operator == DynamoDbFilterOperator.Equal);

            if (numberFilters == 0)
            {
                ArgumentException ex = new ArgumentException($"Not contains any condition of type Equals for field {hashKey}");
                throw ex;
            }
            else if (numberFilters > 1)
            {
                ArgumentException ex = new ArgumentException($"The {hashKey} field only admits one condition");
                throw ex;
            }

            return hashKey;
        }
    }
}
