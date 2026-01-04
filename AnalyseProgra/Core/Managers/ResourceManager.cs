using System;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour ToList()
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using System.Resources;
using AnalyseProgra.Core.Managers;
using System.Xml.Serialization;

public class ResourceManager
{
    private readonly object _verrou = new object();

    public List<ColonyResource> Resources { get; set; }
    public Dictionary<ResourceType, int> MaxCapacities { get; set; }

    public ResourceManager(ICollection<ColonyResource>? colonyResources = null)
    {
        Resources = colonyResources != null ? colonyResources.ToList() : new List<ColonyResource>();
        MaxCapacities = new Dictionary<ResourceType, int>();

        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            MaxCapacities[type] = 100;
        }
    }

    private ColonyResource GetOrCreateResource(ResourceType type)
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

    public static string ResourceListToString(ICollection<ColonyResource> resources)
    {     
        string s;

        if (resources.Count == 1)
        {
            var resource = resources.First();
            s = $"{resource.Quantity} {resource.ResourceType}";
        }
        else
        {
            s = string.Join(", ", resources.Select(r => $"{r.Quantity} {r.ResourceType}"));
        }

        return s;
    }

	public void UpdateMaxStorage(BuildingManager buildingManager)
	{
		lock (_verrou)
		{
			// 1. On remet tout le monde au minimum (base)
			foreach (var type in MaxCapacities.Keys.ToList())
			{
				MaxCapacities[type] = 100;
			}

			if (buildingManager.BuildingStacks != null)
			{
				foreach (var stack in buildingManager.BuildingStacks)
				{
					int capacite = stack.Level * 100 * stack.Amount;

					switch (stack.BuildingType)
					{
						case BuildingType.StorageSaliBuilding:
							if (MaxCapacities.ContainsKey(ResourceType.Sali))
								MaxCapacities[ResourceType.Sali] += capacite;
							break;

						case BuildingType.StorageSolurialBuilding:
							if (MaxCapacities.ContainsKey(ResourceType.Solurial))
								MaxCapacities[ResourceType.Solurial] += capacite;
							break;

						case BuildingType.StorageSoluroBuilding:
							if (MaxCapacities.ContainsKey(ResourceType.Soluro))
								MaxCapacities[ResourceType.Soluro] += capacite;
							break;

						case BuildingType.StorageSoluBuilding:
							if (MaxCapacities.ContainsKey(ResourceType.Solu))
								MaxCapacities[ResourceType.Solu] += capacite;
							break;

						case BuildingType.StorageSoliBuilding:
							if (MaxCapacities.ContainsKey(ResourceType.Soli))
								MaxCapacities[ResourceType.Soli] += capacite;
							break;
					}
				}
			}
		}
	}

	public void Ajouter(ResourceType type, int quantite)
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

    public void Retirer(ResourceType type, int quantite)
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

    public void Retirer(ICollection<ColonyResource> requiredResources)
    {
        lock (_verrou)
        {
            foreach (var required in requiredResources)
            {
                Retirer(required.ResourceType, required.Quantity);
            }
        }
    }

    public bool HasEnough(ResourceType type, int quantite)
    {
        lock (_verrou)
        {
            var res = Resources.FirstOrDefault(r => r.ResourceType == type);

            double qte = res != null ? res.Quantity : 0;

            return qte >= quantite;
        }
    }

    public bool HasEnough(ICollection<ColonyResource> requiredResources)
    {
        lock (_verrou)
        {
            foreach (var required in requiredResources)
            {
                if (HasEnough(required.ResourceType, required.Quantity) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public int GetStock(ResourceType type)
    {
        lock (_verrou)
        {
            var res = Resources.FirstOrDefault(r => r.ResourceType == type);
            return res != null ? (int)res.Quantity : 0;
        }
    }

    public int GetMax(ResourceType type)
    {
        lock (_verrou)
        {
            if (!MaxCapacities.ContainsKey(type)) return 100;
            return MaxCapacities[type];
        }
    }
}
