using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models
{
    public class ColonyResource
    {
        public int ColonyId { get; set; }
        public ResourceType ResourceType { get; set; }
        public int Quantity { get; set; }
        public Colony? Colony { get; set; }


        public ColonyResource(ResourceType resourceType, int quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}
