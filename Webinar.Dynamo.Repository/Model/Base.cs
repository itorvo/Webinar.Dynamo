namespace Webinar.Dynamo.Repository.Model
{
    public abstract class Base
    {
        protected Base()
        {

        }

        internal static string GetPropertyReference(string propertyName) => $"#{char.ToLowerInvariant(propertyName[0])}{propertyName[1..]}";
    }
}
