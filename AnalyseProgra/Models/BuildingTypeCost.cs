namespace AnalyseProgra.Models
{
    public class BuildingTypeCost
    {
        public string BuildingTypeId { get; set; } = null!;
        public string ResourceTypeId { get; set; } = null!;
        public double Amount { get; set; }
    }
}
