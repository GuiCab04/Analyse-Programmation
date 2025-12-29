using System;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour ToList()
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;


public class ResourceManager
{
    private readonly object _verrou = new object();

    private Colony _colony; // Référence vers l'objet DB
    private Dictionary<ResourceTypeEnums, int> _maxCapacities;

    public ResourceManager(Colony colony)
    {
        _colony = colony;
        _maxCapacities = new Dictionary<ResourceTypeEnums, int>();

        foreach (ResourceTypeEnums type in Enum.GetValues(typeof(ResourceTypeEnums)))
        {
            _maxCapacities[type] = 100;
        }

        if (_colony.Resources == null)
        {
            _colony.Resources = new List<ColonyResource>();
        }
    }

    private ColonyResource GetOrCreateResource(ResourceTypeEnums type)
    {
        string typeName = type.ToString();

        var resourceEntity = _colony.Resources.FirstOrDefault(r => r.ResourceType == type);

        if (resourceEntity == null)
        {
            resourceEntity = new ColonyResource(type, 0)
            {
                ColonyId = _colony.Id,
                Colony = _colony
            };
            _colony.Resources.Add(resourceEntity);
        }

        return resourceEntity;
    }

    public void UpdateMaxStorage()
    {
        lock (_verrou)
        {
            foreach (var type in _maxCapacities.Keys.ToList())
            {
                _maxCapacities[type] = 100;
            }

            if (_colony.BuildingStacks != null)
            {
                foreach (var stack in _colony.BuildingStacks)
                {
                    switch (stack.BuildingType)
                    {
                        case BuildingType.StorageBuilding:
                            int capaciteWarehouse = stack.Level * 100 * stack.Amount;

                            foreach (var type in _maxCapacities.Keys.ToList())
                            {
                                _maxCapacities[type] += capaciteWarehouse;
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
            int max = _maxCapacities.ContainsKey(type) ? _maxCapacities[type] : 100;

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
            var res = _colony.Resources.FirstOrDefault(r => r.ResourceType == type);

            double qte = res != null ? res.Quantity : 0;

            return qte >= quantite;
        }
    }

    public int GetStock(ResourceTypeEnums type)
    {
        lock (_verrou)
        {
            var res = _colony.Resources.FirstOrDefault(r => r.ResourceType == type);
            return res != null ? (int)res.Quantity : 0;
        }
    }

    public int GetMax(ResourceTypeEnums type)
    {
        lock (_verrou)
        {
            if (!_maxCapacities.ContainsKey(type)) return 100;
            return _maxCapacities[type];
        }
    }
}
