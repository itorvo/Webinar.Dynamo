using System;
using System.Runtime.Serialization;

namespace Webinar.Dynamo.Repository.Exceptions
{
    [Serializable]
    public class TableAttributeException : Exception
    {
        public TableAttributeException()
            : base("Type does not have attribute DynamoDBTableAttribute")
        { }

        public TableAttributeException(Type type)
            : base($"Type '{type}' does not have attribute DynamoDBTableAttribute")
        { }

        protected TableAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}