using Webinar.Dynamo.Repository.Enumerators;

namespace Webinar.Dynamo.Repository.Filters
{
    public class FilterQuery
    {
        public string AtributeName { get; set; }
        public object ValueAtribute { get; set; }
        public int Operator { get; set; }
        public object ValueAtributeFinal { get; set; }
        public DynamoDbTypeCondition TypeCondition { get; set; }
    }
}
