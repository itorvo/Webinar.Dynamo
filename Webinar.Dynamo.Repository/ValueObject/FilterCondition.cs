using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Webinar.Dynamo.Repository.Converters;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.ValueObject
{
    public class FilterCondition
    {
        public string AtributeName { get; set; }
        public DynamoDbFilterOperator Operator { get; set; }
        public DynamoDbTypeCondition TypeCondition { get; set; }
        public object ValueAtribute { get; set; }

        public static implicit operator Condition(FilterCondition filter)
        {
            Type type;
            List<AttributeValue> attributeValueList;
            IList list;

            switch (filter.Operator)
            {
                case DynamoDbFilterOperator.IsNull:
                case DynamoDbFilterOperator.IsNotNull:
                    return new Condition
                    {
                        ComparisonOperator = filter.Operator.GetComparisonOperator()
                    };
                case DynamoDbFilterOperator.In:
                case DynamoDbFilterOperator.Between:
                    list = (IList)filter.ValueAtribute;
                    attributeValueList = new List<AttributeValue>();
                    foreach (var item in list)
                    {
                        type = item.GetType();
                        attributeValueList.Add(AttributeValueConverter.ConvertToAttributeValue[type](item));
                    }
                    break;
                default:
                    type = filter.ValueAtribute.GetType();
                    attributeValueList = new List<AttributeValue> { AttributeValueConverter.ConvertToAttributeValue[type](filter.ValueAtribute) };
                    break;
            }

            return new Condition
            {
                AttributeValueList = attributeValueList,
                ComparisonOperator = filter.Operator.GetComparisonOperator()
            };
        }
    }
}
