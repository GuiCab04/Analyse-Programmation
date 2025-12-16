namespace AnalyseProgra.Models
{
    public class GameSave
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ColonyId { get; set; }

        public string SaveName { get; set; } = "";
        public string DataJson { get; set; } = "";
    }
}
