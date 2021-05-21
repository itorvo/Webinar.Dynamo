using Amazon.DynamoDBv2.DocumentModel;

namespace Webinar.Dynamo.Repository.Factories
{
    internal interface ISearchFactory
    {
        Search CreateSearch();
    }
}
