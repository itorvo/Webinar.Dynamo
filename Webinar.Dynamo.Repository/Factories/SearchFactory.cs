using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Webinar.Dynamo.Repository.Enumerators;
using Webinar.Dynamo.Repository.Extensions;
using Webinar.Dynamo.Repository.ValueObject;

[assembly: InternalsVisibleTo("Webinar.Dynamo.Repository.Extensions")]

namespace Webinar.Dynamo.Repository.Factories
{
    internal partial class SearchFactory : ISearchFactory
    {
        internal readonly Table Table;
        internal readonly List<FilterCondition> Conditions;
        internal readonly string PaginationToken;
        internal readonly string IndexName;
        internal readonly bool BackwardSearch;
        internal readonly bool GetTotal;
        internal readonly int Limit;
        internal readonly bool IsQueryOperation;
        internal readonly Filter Filter;
        internal readonly OperatorConditional Operator;
        internal List<string> AttributesToGet;

        internal SearchFactory(Table table, FilterRequest request)
        {
            Table = table;
            Conditions = request.Conditions;
            PaginationToken = request.PaginationToken;
            IndexName = request.IndexName;
            BackwardSearch = request.BackwardSearch;
            AttributesToGet = request.AttributesToGet;
            GetTotal = request.GetTotal;
            Limit = request.Limit;
            Operator = request.Operator;

            IsQueryOperation = Conditions.Any(c => c.TypeCondition == DynamoDbTypeCondition.Query);

            Filter = IsQueryOperation ? new QueryFilter() : (Filter)new ScanFilter();
        }

        public Search CreateSearch()
        {
            SetConditions();
            return IsQueryOperation ? CreateSearchQueryOperationConfig() : CreateSearchScanOperationConfig();
        }

        private Search CreateSearchQueryOperationConfig()
        {
            var query = new QueryOperationConfig
            {
                Filter = (QueryFilter)Filter,
                BackwardSearch = BackwardSearch,
                ConditionalOperator = (ConditionalOperatorValues)Operator,
                CollectResults = true,
                PaginationToken = !string.IsNullOrEmpty(PaginationToken) && !PaginationToken.Equals("{}") ? PaginationToken : null,
                IndexName = !string.IsNullOrEmpty(IndexName) ? IndexName : null,
                Limit = Limit > 0 ? Limit : int.MaxValue
            };

            DefineAttributesToGet();

            if (AttributesToGet != null && AttributesToGet.Any())
            {
                query.AttributesToGet = AttributesToGet;
                query.Select = SelectValues.SpecificAttributes;
            }

            return Table.Query(query);
        }

        private Search CreateSearchScanOperationConfig()
        {
            var scan = new ScanOperationConfig
            {
                Filter = (ScanFilter)Filter,
                CollectResults = true,
                ConditionalOperator = (ConditionalOperatorValues)Operator,
                PaginationToken = !string.IsNullOrEmpty(PaginationToken) && !PaginationToken.Equals("{}") ? PaginationToken : null,
                IndexName = !string.IsNullOrEmpty(IndexName) ? IndexName : null,
                Limit = Limit > 0 ? Limit : int.MaxValue
            };

            DefineAttributesToGet();

            if (AttributesToGet != null && AttributesToGet.Any())
            {
                scan.AttributesToGet = AttributesToGet;
                scan.Select = SelectValues.SpecificAttributes;
            }

            return Table.Scan(scan);
        }

        private void DefineAttributesToGet()
        {
            if (GetTotal)
            {
                AttributesToGet = Filter.ToConditions().Keys.Distinct().ToList();
            }
        }

        private void SetConditions()
        {
            if (!ValidateConditions(out string hashKey))
            {
                ArgumentException ex = new ArgumentException($"Not contains any condition of type Equals for field {hashKey}");
                throw ex;
            }

            Conditions.ForEach(f => Filter.AddCondition(f.AtributeName, f));
        }

        private bool ValidateConditions(out string hashKey)
        {
            this.ValidDuplicateCondition();

            this.ValidBetweenCondition();

            this.ValidInCondition();

            hashKey = IsQueryOperation ? this.ValidIndexAndKeys() : null;

            return true;
        }
    }
}
