namespace Webinar.Dynamo.ConsoleApp.Entities
{
    public class State
    {
        public string Country { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public int NumberCitizens { get; set; }

        public override string ToString()
        {
            return $"{Country}\t\t{Code}\t\t{Name}\t\t{NumberCitizens}";
        }
    }

}
