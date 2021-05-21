using System;
using System.Runtime.Serialization;
using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.Exceptions
{
    [Serializable]
    public class TableKeyAttributeException : Exception
    {
        public TableKeyAttributeException()
            : base("Type does not have any key attributes defined")
        { }

        protected TableKeyAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal TableKeyAttributeException(Type type)
            : base($"Type '{type}' does not have any key attributes defined")
        { }

        internal TableKeyAttributeException(Type type, EnumKeyType keys)
            : base($"Type '{type}' can only have 1 {keys} defined")
        { }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}