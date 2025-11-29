using System.Collections.Generic;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.Models.Buildings
{
   
    public abstract class Building
    {
        public int Id { get; private set; }
        public string Nom { get; protected set; }
        public int Level { get; protected set; }

        public Building(int id, string nom)
        {
            Id = id;
            Nom = nom;
            Level = 1;
        }

        public abstract Dictionary<ResourceTypeEnums, int> GetUpgradeCost();

        public bool TryUpgrade(ResourceManager resourceManager)
        {
            var cout = GetUpgradeCost();

            foreach (var item in cout)
            {
                if (!resourceManager.HasEnough(item.Key, item.Value))
                {
                    return false; 
                }
            }

            foreach (var item in cout)
            {
                resourceManager.Retirer(item.Key, item.Value);
            }

            Level++;
            ApplyUpgradeEffect();
            return true;
        }

        protected abstract void ApplyUpgradeEffect();
    }
}