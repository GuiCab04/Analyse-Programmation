using System;
using System.Collections.Generic;
using AnalyseProgra.Core.Managers;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;

public class PopulationManager
{
    private readonly object _verrou = new object();

    public int MaxPopulation { get; set; }
    public int CurrentPopulation { get; set; }

    public PopulationManager(int populationCount = 0)
    {
        MaxPopulation = 0;
        CurrentPopulation = populationCount;
    }

   
    public void UpdateMaxPopulation(BuildingManager buildingManager)
    {
        lock (_verrou)
        {
            int totalCapacite = 0;

            var houseStacks = buildingManager.BuildingStacks.Where(b => b.BuildingType == BuildingType.HousingBuilding);

            if (houseStacks != null)
            {
                foreach (var stack in houseStacks)
                {
                    totalCapacite += stack.Level * 3 * stack.Amount;
                }
            }            

            MaxPopulation = totalCapacite;
        }
    }

    public void Ajouter(int quantite)
    {
        lock (_verrou)
        {
            if (CurrentPopulation + quantite <= MaxPopulation)
            {
                CurrentPopulation += quantite;
            }
            else
            {
                CurrentPopulation = MaxPopulation;
            }
        }
    }

    public void Retirer(int quantite)
    {
        lock (_verrou)
        {
            if (CurrentPopulation - quantite >= 0)
            {
                CurrentPopulation -= quantite;
            }
            else
            {
                CurrentPopulation = 0;
            }
        }
    }

    public bool HasEnough(int quantite)
    {
        lock (_verrou)
        {
            return CurrentPopulation >= quantite;
        }
    }

    public int GetStock()
    {
        lock (_verrou)
        {
            return CurrentPopulation;
        }
    }

    public string GetStatusString()
    {
        lock (_verrou)
        {
            return $"{CurrentPopulation} / {MaxPopulation}";
        }
    }
}