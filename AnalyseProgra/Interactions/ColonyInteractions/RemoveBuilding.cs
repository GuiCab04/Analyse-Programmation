using AnalyseProgra.Core.Managers;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public class RemoveBuilding : ColonyInteraction
    {
        public RemoveBuilding(Colony colony)
            : base(colony, "Supprimer un bâtiment", "Retirer une unité d'un type de bâtiment présent dans la colonie.")
        {
        }

        public override void Execute(IUserInterface input)
        {
            if (_colony.Buildings == null || _colony.Buildings.BuildingStacks.Count == 0)
            {
                input.WriteError("Aucun bâtiment à supprimer.");
                input.Pause();
                return;
            }

            var stacks = _colony.Buildings.BuildingStacks.ToList();
            var choices = stacks.Select((b, i) => $"{i} - {BuildingManager.GetBuildingName(b.BuildingType)} (Niv {b.Level}) x{b.Amount}").ToList();
            choices.Add("[red]Retour[/]");

            var selection = input.Select("Quel bâtiment voulez‑vous supprimer ?", choices);
            if (selection == "[red]Retour[/]") return;

            if (!int.TryParse(selection.Split('-')[0].Trim(), out int index) || index < 0 || index >= stacks.Count)
            {
                input.WriteError("Sélection invalide.");
                input.Pause();
                return;
            }

            var stackSelectionne = stacks[index];
			if (stackSelectionne.BuildingType == BuildingType.Mairie)
			{
				input.WriteMessage("[red]Impossible : la Mairie est un bâtiment unique et ne peut pas être supprimée.[/]");
				input.Pause();
				return;
			}

			if (!input.Confirm($"Confirmer la suppression d'une unité de {BuildingManager.GetBuildingName(stackSelectionne.BuildingType)} (Niv {stackSelectionne.Level}) ?"))
            {
                return;
            }

            try
            {
                _colony.Buildings.RemoveBuilding(stackSelectionne.BuildingType, stackSelectionne.Level);
                input.WriteMessage("[green]Suppression effectuée.[/]");
            }
            catch (InvalidOperationException ex)
            {
                input.WriteError($"Suppression impossible : {ex.Message}");
            }

            input.Pause();
        }
    }
}