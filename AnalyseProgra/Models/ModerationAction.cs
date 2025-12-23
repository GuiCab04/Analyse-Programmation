namespace AnalyseProgra.Models
{
    public class ModerationAction
    {
        public int Id { get; set; }
        public int PerformedByUserId { get; set; }
        public int TargetUserId { get; set; }
        public string ActionType { get; set; } = "";
        public string? Details { get; set; }
    }
}
