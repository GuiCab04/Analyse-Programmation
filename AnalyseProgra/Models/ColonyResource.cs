using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models
{
    public class ColonyResource
    {
        public int ColonyId { get; set; }
        public ResourceTypeEnums ResourceType { get; set; }
        public double Quantity { get; set; }
        public Colony? Colony { get; set; }


        public ColonyResource(ResourceTypeEnums resourceType, double quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
        }
    }
}
