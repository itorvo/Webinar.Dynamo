using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Webinar.Dynamo.Repository.Converters
{
    internal static class ListAttributeValueConverter
    {
        internal static readonly Type[] NullableNumberTypes = {
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
            typeof(short?),
            typeof(int?),
            typeof(long?),
            typeof(ushort?),
            typeof(uint?),
            typeof(ulong?),
        };

        internal static readonly Type[] NumberTypes = {
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
        };

        internal static readonly Type[] StringTypes = {
            typeof(string)
        };

        internal static Type[] AllNumberTypes
        {
            get
            {
                return GetAllNumberTypes();
            }
        }

        private static Type[] GetAllNumberTypes()
        {
            return NumberTypes
                    .Concat(NullableNumberTypes)
                    .ToArray();
        }

        internal static Type[] AllTypes
        {
            get
            {
                return GetAllTypes();
            }
        }

        private static Type[] GetAllTypes()
        {
            return StringTypes
                    .Concat(NumberTypes)
                    .Concat(NullableNumberTypes)
                    .ToArray();
        }

        internal static AttributeValue ConvertToAttributeValue(Type type, IEnumerator listValues)
        {
            var stringValues = new List<string>();
            var attributeValues = new List<AttributeValue>();

            if (listValues == null)
            {
                return new AttributeValue { NULL = true };
            }

            if (AllTypes.Contains(type))
            {
                while (listValues.MoveNext())
                {
                    stringValues.Add($"{listValues.Current}");
                }
            }
            else if (type.IsClass)
            {
                while (listValues.MoveNext())
                {
                    attributeValues.Add(AttributeValueConverter.ConvertToAttributeValue[listValues.Current.GetType()](listValues.Current));
                }
            }
            else
            {
                throw new InvalidCastException("Type not supported: " + type.Name);
            }

            return GetAttributeValue(type, stringValues, attributeValues);
        }

        internal static AttributeValue ConvertToAttributeValue(Type type, IList listValues)
        {
            var stringValues = new List<string>();
            var attributeValues = new List<AttributeValue>();

            if (listValues == null)
            {
                return new AttributeValue { NULL = true };
            }

            if (AllTypes.Contains(type))
            {
                foreach (var current in listValues)
                {
                    stringValues.Add($"{current}");
                }
            }
            else if (type.IsClass)
            {
                foreach (var current in listValues)
                {
                    attributeValues.Add(AttributeValueConverter.ConvertToAttributeValue[current.GetType()](current));
                }
            }
            else
            {
                throw new InvalidCastException("Type not supported: " + type.Name);
            }

            return GetAttributeValue(type, stringValues, attributeValues);
        }

        private static AttributeValue GetAttributeValue(Type type, List<string> stringValues, List<AttributeValue> attributeValues)
        {
            if (stringValues.Count > 0)
            {
                if (StringTypes.Contains(type))
                {
                    return new AttributeValue { SS = stringValues };
                }
                else if (AllNumberTypes.Contains(type))
                {
                    return new AttributeValue { NS = stringValues };
                }
                else
                {
                    throw new InvalidCastException("Type not supported: " + type.Name);
                }
            }
            else if (attributeValues.Count > 0)
            {
                return new AttributeValue { L = attributeValues };
            }
            else
            {
                throw new InvalidCastException("Type not supported: " + type.Name);
            }
        }
    }
}
