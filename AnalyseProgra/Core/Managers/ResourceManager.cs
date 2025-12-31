using System;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour ToList()
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using System.Resources;
using AnalyseProgra.Core.Managers;

public class ResourceManager
{
    private readonly object _verrou = new object();

    public List<ColonyResource> Resources { get; set; }
    public Dictionary<ResourceTypeEnums, int> MaxCapacities { get; set; }

    public ResourceManager(ICollection<ColonyResource>? colonyResources = null)
    {
        Resources = colonyResources != null ? colonyResources.ToList() : new List<ColonyResource>();
        MaxCapacities = new Dictionary<ResourceTypeEnums, int>();

        foreach (ResourceTypeEnums type in Enum.GetValues(typeof(ResourceTypeEnums)))
        {
            MaxCapacities[type] = 100;
        }
    }

    private ColonyResource GetOrCreateResource(ResourceTypeEnums type)
    {
        string typeName = type.ToString();

        var resourceEntity = Resources.FirstOrDefault(r => r.ResourceType == type);

        if (resourceEntity == null)
        {
            resourceEntity = new ColonyResource(type, 0);
            Resources.Add(resourceEntity);
        }

        return resourceEntity;
    }

    public void UpdateMaxStorage(BuildingManager buildingManager)
    {
        lock (_verrou)
        {
            foreach (var type in MaxCapacities.Keys.ToList())
            {
                MaxCapacities[type] = 100;
            }

            if (buildingManager.BuildingStacks != null)
            {
                foreach (var stack in buildingManager.BuildingStacks)
                {
                    switch (stack.BuildingType)
                    {
                        case BuildingType.StorageBuilding:
                            int capaciteWarehouse = stack.Level * 100 * stack.Amount;

                            foreach (var type in MaxCapacities.Keys.ToList())
                            {
                                MaxCapacities[type] += capaciteWarehouse;
                            }
                            break;

                            // Possibilité d'ajouter d'autres bâtiments influençant la capacité de stockage
                    }
                }
            }
        }
    }

    public void Ajouter(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            var resourceEntity = GetOrCreateResource(type);

            int stockActuel = (int)resourceEntity.Quantity;
            int max = MaxCapacities.ContainsKey(type) ? MaxCapacities[type] : 100;

            int futurStock = stockActuel + quantite;
            if (futurStock > max) futurStock = max;

            if (futurStock != stockActuel)
            {
                resourceEntity.Quantity = futurStock;
            }
        }
    }

    public void Retirer(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            var resourceEntity = GetOrCreateResource(type);

            int stockActuel = (int)resourceEntity.Quantity;
            int futurStock = stockActuel - quantite;

            if (futurStock < 0) futurStock = 0;

            if (futurStock != stockActuel)
            {
                resourceEntity.Quantity = futurStock;
            }
        }
    }

    public bool HasEnough(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            var res = Resources.FirstOrDefault(r => r.ResourceType == type);

            double qte = res != null ? res.Quantity : 0;

            return qte >= quantite;
        }
    }

    public int GetStock(ResourceTypeEnums type)
    {
        lock (_verrou)
        {
            var res = Resources.FirstOrDefault(r => r.ResourceType == type);
            return res != null ? (int)res.Quantity : 0;
        }
    }

    public int GetMax(ResourceTypeEnums type)
    {
        lock (_verrou)
        {
            if (!MaxCapacities.ContainsKey(type)) return 100;
            return MaxCapacities[type];
        }
    }
}
