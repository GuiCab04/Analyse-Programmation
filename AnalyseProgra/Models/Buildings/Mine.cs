using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
    public class Mine : ProductionBuilding
    {
        public Mine(ResourceTypeEnums typeMinerai)
            : base($"Mine de {typeMinerai}", typeMinerai)
        {
            // On force le recalcul dès la création pour appliquer les taux corrects
            ApplyUpgradeEffect();
        }

        // C'est ici qu'on définit la différence entre Or et Fer sans créer 2 classes !
        protected override void ApplyUpgradeEffect()
        {
            if (ResourceProduite == ResourceTypeEnums.Or)
            {
                // L'Or est rare : Production faible (1, 2, 3...)
                TauxProduction = Level * 1;
            }
            else
            {
                // Le Fer est abondant : Production standard (5, 10, 15...)
                TauxProduction = Level * 5;
            }
        }

        public override Dictionary<ResourceTypeEnums, int> GetUpgradeCost()
        {
            if (ResourceProduite == ResourceTypeEnums.Or)
            {
                // Une mine d'Or coûte cher et demande du Fer !
                return new Dictionary<ResourceTypeEnums, int>
                {
                    { ResourceTypeEnums.Fer, Level * 200 }
                };
            }
            else
            {
                // Une mine de Fer coûte un peu de Fer
                return new Dictionary<ResourceTypeEnums, int>
                {
                    { ResourceTypeEnums.Fer, Level * 50 }
                };
            }
        }
    }
}