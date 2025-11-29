using System;
using System.Collections.Generic;
using AnalyseProgra.Models.Buildings;

public class PopulationManager
{
    private readonly object _verrou = new object();

    private int _currentPopulation;
    private int _maxPopulation;

    public PopulationManager()
    {
        _currentPopulation = 2; 
        _maxPopulation = 0;
    }

   
    public void UpdateMaxPopulation(List<Building> batiments)
    {
        lock (_verrou)
        {
            int totalCapacite = 0;

            foreach (var b in batiments)
            {
                if (b is HousingBuilding maison)
                {
                    totalCapacite += maison.CapaciteHabitants;
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