using System;
using System.Collections.Generic;
using System.Linq; // Nécessaire pour ToList()
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Buildings; // Nécessaire pour voir "StorageBuilding"


public class ResourceManager
{
    private readonly object _verrou = new object();

    private Dictionary<ResourceTypeEnums, int> _stock;
    private Dictionary<ResourceTypeEnums, int> _maxCapacities;

    public ResourceManager()
    {
        _stock = new Dictionary<ResourceTypeEnums, int>();
        _maxCapacities = new Dictionary<ResourceTypeEnums, int>();

        // C'EST ICI QUE LES MAX SONT DÉFINIS !
        foreach (ResourceTypeEnums type in Enum.GetValues(typeof(ResourceTypeEnums)))
        {
            _stock[type] = 0;
            _maxCapacities[type] = 100; // <--- VALEUR PAR DÉFAUT (100)
        }
    }

    // --- NOUVELLE MÉTHODE POUR LES BATIMENTS DE STOCKAGE ---
    public void UpdateMaxStorage(List<Building> batiments)
    {
        lock (_verrou)
        {
            // 1. On remet tout à la valeur de base (100)
            // On utilise ToList() pour éviter de modifier la collection qu'on parcourt si nécessaire
            foreach (var type in _maxCapacities.Keys.ToList())
            {
                _maxCapacities[type] = 100;
            }

            // 2. On ajoute la capacité de chaque bâtiment de stockage
            foreach (var b in batiments)
            {
                if (b is StorageBuilding entrepot)
                {
                    // Si TypeStockage est null, c'est un Entrepôt Général (valable pour tout)
                    if (entrepot.TypeStockage == null)
                    {
                        foreach (var type in _maxCapacities.Keys.ToList())
                        {
                            _maxCapacities[type] += entrepot.CapaciteAjoutee;
                        }
                    }
                    // Sinon, c'est un Silo spécifique (ex: juste pour le Fer)
                    else if (_maxCapacities.ContainsKey(entrepot.TypeStockage.Value))
                    {
                        _maxCapacities[entrepot.TypeStockage.Value] += entrepot.CapaciteAjoutee;
                    }
                }
            }
        }
    }

    public void Ajouter(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            int stockActuel = _stock[type];
            int max = _maxCapacities[type];

            if (stockActuel + quantite < max)
            {
                _stock[type] += quantite;
            }
            else
            {
                _stock[type] = max; // On plafonne
            }
        }
    }

    public void Retirer(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            if (_stock[type] - quantite >= 0)
            {
                _stock[type] -= quantite;
            }
            else
            {
                _stock[type] = 0;
            }
        }
    }

    public bool HasEnough(ResourceTypeEnums type, int quantite)
    {
        lock (_verrou)
        {
            return _stock[type] >= quantite;
        }
    }

    public int GetStock(ResourceTypeEnums type)
    {
        lock (_verrou)
        {
            if (!_stock.ContainsKey(type)) return 0;
            return _stock[type];
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

    // Ancienne méthode manuelle (peut être gardée ou supprimée)
    public void SetMaxCapacity(ResourceTypeEnums type, int newMax)
    {
        lock (_verrou)
        {
            _maxCapacities[type] = newMax;
        }
    }
}
