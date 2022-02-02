using Amazon.DynamoDBv2.DataModel;

namespace Webinar.Dynamo.Domain.Entities
{
    //[DynamoDBTable("Location")]
    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
