namespace AnalyseProgra.Models
{
    public class ColonyResource
    {
        public int ColonyId { get; set; }
        public string ResourceName { get; set; } = "";
        public double Quantity { get; set; }
        public Colony? Colony { get; set; }
    }
}
