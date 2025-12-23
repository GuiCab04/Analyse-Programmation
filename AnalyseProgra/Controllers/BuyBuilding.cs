using AnalyseProgra.Models.Users;
using AnalyseProgra.UserInterfaces;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Controllers
{
    public class BuyBuilding : Interaction
    {
        private Player _player;
        private Dictionary<string, Func<Building>> _buildings;

        public BuyBuilding(Player player)
            : base("Acheter un bâtiment", "Permet d'acheter un bâtiment.")
        {
            _player = player;

            _buildings = new Dictionary<string, Func<Building>>
            {
                { "Ferme",              () => new Ferme() },
                { "Housing building",   () => new HousingBuilding() },
                { "Mine de fer",        () => new Mine(ResourceTypeEnums.Fer) },
                { "Mine d'or",          () => new Mine(ResourceTypeEnums.Or) },
                { "Storage building",   () => new StorageBuilding() }
            };
        }

        public override void Execute(IUserInterface input)
        {
            input.WriteTitle($"Construction - {_player.Name}");

            string buildingType = input.Select("Quel type de bâtiment ?", _buildings.Keys);
            Building building = _buildings[buildingType].Invoke();

            int buildingCount = input.Ask<int>($"Combien de {buildingType}(s) voulez-vous ?");

            _player.AddBuilding(building);
        }
    }
}
