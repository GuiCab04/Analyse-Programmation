namespace AnalyseProgra.Models
{
    public class Colony
    {
        public int Id { get; set; }
        public string OwnerUsername { get; set; } = "";
        public string Name { get; set; } = "";
        public int PopulationCount { get; set; }
        public double Morale { get; set; }

        public User? Owner { get; set; }
        public ICollection<ColonyBuildingStack>? BuildingStacks { get; set; }
        public ICollection<ColonyResource>? Resources { get; set; }

        public Colony()
        {
            BuildingStacks = new List<ColonyBuildingStack>();
            Resources = new List<ColonyResource>();
        }
    }
}
