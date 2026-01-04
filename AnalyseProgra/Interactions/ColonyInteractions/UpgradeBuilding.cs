using AnalyseProgra.Core.Managers;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public class UpgradeBuilding : ColonyInteraction
    {
        public UpgradeBuilding(Colony colony)
            : base(colony, "Améliorer un bâtiment", "Permet d'améliorer le niveau d'un bâtiment existant dans la colonie.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            List<ColonyBuildingStack> upgradeableStacks;

            if (_colony.Buildings == null || _colony.Buildings.BuildingStacks.Count == 0) {
                input.WriteError("Aucun bâtiment.");
                input.Pause();
                return;
            }
            // On convertit en liste pour pouvoir indexer
            upgradeableStacks = _colony.Buildings.BuildingStacks.ToList();

            // Création du menu de sélection
            var choices = upgradeableStacks.Select((b, i) => $"{i} - {b.BuildingType} (Niv {b.Level}) x{b.Amount}").ToList();
            choices.Add("[red]Retour[/]");

            var selection = input.Select("Quel type de bâtiments améliorer ?", choices);
            if (selection == "[red]Retour[/]") return;

            int index = int.Parse(selection.Split('-')[0].Trim());
            ColonyBuildingStack stackSelectionne = upgradeableStacks[index];

            var cost = stackSelectionne.GetUpgradeCost();
            var costString = cost == null ? "Gratuit" : ResourceManager.ResourceListToString(cost);

            input.WriteMessage($"Coût pour améliorer ce building au niveau {stackSelectionne.Level + 1}:");
            input.WriteMessage($"- {costString}");

            if (input.Confirm($"Confirmer l'amélioration pour {costString} Soluro ?"))
            {
                if (_colony.Resources.HasEnough(cost))
                {
                    _colony.Resources.Retirer(cost);
                    _colony.Buildings.UpgradeBuilding(stackSelectionne.BuildingType, stackSelectionne.Level);

                    // Mise à jour des calculs
                    _colony.Population.UpdateMaxPopulation(_colony.Buildings);
                    _colony.Resources.UpdateMaxStorage(_colony.Buildings);

                    input.WriteMessage("[green]Succès ! Niveau augmenté.[/]");
                }
                else input.WriteError("Pas assez de ressources.");
            }
            input.Pause();
        }
    }
}
