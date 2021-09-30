using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace Webinar.Dynamo.Domain.Entities
{
    [DynamoDBTable("Location")]
    public class State
    {
        [DynamoDBHashKey]
        [Required]
        public string Country { get; set; }

        [DynamoDBRangeKey]
        [Required]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int NumberCitizens { get; set; }
    }
}
