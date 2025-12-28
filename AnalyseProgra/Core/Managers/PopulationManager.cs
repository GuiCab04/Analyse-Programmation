using System;
using System.Collections.Generic;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;

public class PopulationManager
{
    private readonly object _verrou = new object();

    private Colony _colony;
    private int _maxPopulation;

    private int _currentPopulation
    {
        get { return _colony.PopulationCount; }
        set { _colony.PopulationCount = value; }
    }

    public PopulationManager(Colony colony)
    {
        _colony = colony;
        _maxPopulation = 0;
    }

   
    public void UpdateMaxPopulation()
    {
        lock (_verrou)
        {
            int totalCapacite = 0;

            var houseStacks = _colony.BuildingStacks.Where(b => b.BuildingType == BuildingType.House);

            if (houseStacks != null)
            {
                foreach (var stack in houseStacks)
                {
                    totalCapacite += stack.Level * 3 * stack.Amount;
                }
            }            

            _maxPopulation = totalCapacite;
        }
    }

    public void Ajouter(int quantite)
    {
        lock (_verrou)
        {
            if (_currentPopulation + quantite <= _maxPopulation)
            {
                _currentPopulation += quantite;
            }
            else
            {
                _currentPopulation = _maxPopulation;
            }
        }
    }

    public void Retirer(int quantite)
    {
        lock (_verrou)
        {
            if (_currentPopulation - quantite >= 0)
            {
                _currentPopulation -= quantite;
            }
            else
            {
                _currentPopulation = 0;
            }
        }
    }

    public bool HasEnough(int quantite)
    {
        lock (_verrou)
        {
            return _currentPopulation >= quantite;
        }
    }

    public int GetStock()
    {
        lock (_verrou)
        {
            return _currentPopulation;
        }
    }

    public string GetStatusString()
    {
        lock (_verrou)
        {
            return $"{_currentPopulation} / {_maxPopulation}";
        }
    }
}