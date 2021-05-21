using System;

namespace Webinar.Dynamo.Repository.Helpers
{
    public class SearchTotalRecordsAttribute : Attribute
    {
        public bool GetTotal { get; set; }

        public SearchTotalRecordsAttribute(bool getTotal)
        {
            GetTotal = getTotal;
        }
    }
}
