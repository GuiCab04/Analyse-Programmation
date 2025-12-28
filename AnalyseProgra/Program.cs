using AnalyseProgra.DataAccess.Dao;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;
using System;
using System.Linq;

namespace AnalyseProgra
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserDao userDao = new UserDao();
            IColonyDao colonyDao = new ColonyDao();
            IColonyBuildingStackDao buildingDao = new ColonyBuildingStackDao();
            IColonyResourceDao resourceDao = new ColonyResourceDao();

            var admin = userDao.GetByUsername("admin")
                ?? userDao.Create(new User
                {
                    Username = "admin",
                    PasswordHash = "pass123",
                    Role = UserRole.Admin,
                    IsActive = true
                });

            var colonies = colonyDao.GetByOwner(admin.Username, includeDetails: true).ToList();

            if (!colonies.Any())
            {
                var createdColony = colonyDao.Create(new Colony
                {
                    OwnerUsername = admin.Username,
                    Name = "New Hope",
                    PopulationCount = 10,
                    Morale = 100
                });

                colonies = colonyDao.GetByOwner(admin.Username, includeDetails: true).ToList();
            }

            var colony = colonies.First();

            var farmStack = buildingDao.GetOne(colony.Id, BuildingType.Farm, 1)
                ?? buildingDao.Create(new ColonyBuildingStack
                {
                    ColonyId = colony.Id,
                    BuildingType = BuildingType.Farm,
                    Level = 1,
                    Amount = 2
                });

            var foodRes = resourceDao.GetOne(colony.Id, "Food")
                ?? resourceDao.Create(new ColonyResource
                {
                    ColonyId = colony.Id,
                    ResourceName = "Food",
                    Quantity = 150
                });

            var loadedUser = userDao.GetById(admin.Id)!;
            loadedUser.Colonies = colonyDao.GetByOwner(loadedUser.Username, includeDetails: true).ToList();

            Console.WriteLine("=== TEST OO ===");
            Console.WriteLine($"{loadedUser.Username} ({loadedUser.Role})");

            foreach (var c in loadedUser.Colonies)
            {
                Console.WriteLine($"Colony: {c.Name}");

                Console.WriteLine("  Buildings:");
                if (c.BuildingStacks != null)
                {
                    foreach (var b in c.BuildingStacks)
                        Console.WriteLine($"    {b.BuildingType} L{b.Level} x{b.Amount}");
                }

                Console.WriteLine("  Resources:");
                if (c.Resources != null)
                {
                    foreach (var r in c.Resources)
                        Console.WriteLine($"    {r.ResourceName}: {r.Quantity}");
                }
            }

            Console.WriteLine("\n✔ TEST FINI");
            Console.ReadKey();
        }
    }
}




/* using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static ResourceManager _ressources = new ResourceManager();
    static PopulationManager _population = new PopulationManager();

    static List<Building> _batiments = new List<Building>();

    static bool _jeuEnCours = true;

    static async Task Main(string[] args)
    {
        
        _ressources.Ajouter(ResourceTypeEnums.Fer, 200);

        _batiments.Add(new Mine(1, ResourceTypeEnums.Fer));
        _batiments.Add(new Ferme(2));
        _batiments.Add(new HousingBuilding(3));

        _population.UpdateMaxPopulation(_batiments);

        var tacheMoteur = Task.Run(() => BoucleDeJeu());

        while (_jeuEnCours)
        {
            Console.Clear();
            Console.WriteLine("=== COLONY MANAGER 2026 ===");
            Console.WriteLine("Moteur actif : Le temps passe...");
            Console.WriteLine("-----------------------------");

            Console.WriteLine("1. [RAPPORT] Voir État Global");
            Console.WriteLine("2. [INFOS]   Voir mes Bâtiments");
            Console.WriteLine("\n--- GESTION ---");
            Console.WriteLine("3. [CONST]   Construire un Bâtiment");
            Console.WriteLine("4. [UPGRADE] Améliorer un Bâtiment");
            Console.WriteLine("5. [ACTION]  Consommer Ressource");

            Console.WriteLine("\n--- POPULATION ---");
            Console.WriteLine("6. [POP]     Ajouter Colon");
            Console.WriteLine("7. [POP]     Tuer Colon");

            Console.WriteLine("\nQ. Quitter");
            Console.Write("\nVotre choix : ");

            var input = Console.ReadKey(true).Key;
            Console.WriteLine();

            switch (input)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    AfficherEtatGlobal();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    AfficherBatiments();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    ActionConstruireBatiment();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    ActionAmeliorerBatiment();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    ActionRetirerRessource();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    ActionAjouterPopulation();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    ActionRetirerPopulation();
                    PauseUtilisateur();
                    break;

                case ConsoleKey.Q:
                    _jeuEnCours = false;
                    Console.WriteLine("Arrêt du moteur...");
                    break;
            }

            await Task.Delay(50);
        }

        await tacheMoteur;
        Console.WriteLine("Fermeture complète.");
    }

   

    static void ActionAmeliorerBatiment()
    {
        Console.WriteLine("\n=== AMÉLIORATION DES BÂTIMENTS ===");
        if (_batiments.Count == 0)
        {
            Console.WriteLine("Vous n'avez aucun bâtiment à améliorer.");
            return;
        }

        foreach (var b in _batiments)
        {
            Console.WriteLine($"ID {b.Id} - {b.Nom} (Niv {b.Level})");
        }

        Console.Write("\nEntrez l'ID du bâtiment à améliorer : ");
        string input = Console.ReadLine();

        if (int.TryParse(input, out int idRecherche))
        {
            
            Building batiment = _batiments.FirstOrDefault(b => b.Id == idRecherche);

            if (batiment != null)
            {
                
                var couts = batiment.GetUpgradeCost();
                Console.WriteLine($"\nCoût pour passer au niveau {batiment.Level + 1} :");
                foreach (var c in couts)
                {
                    Console.WriteLine($"- {c.Value} {c.Key}");
                }

                Console.Write("Confirmer ? (O/N) : ");
                if (Console.ReadKey().Key == ConsoleKey.O)
                {
                    Console.WriteLine();                    
                    bool succes = batiment.TryUpgrade(_ressources);

                    if (succes)
                    {
                        Console.WriteLine($"[SUCCÈS] {batiment.Nom} est maintenant niveau {batiment.Level} !");

                        _population.UpdateMaxPopulation(_batiments);
                        _ressources.UpdateMaxStorage(_batiments);
                    }
                    else
                    {
                        Console.WriteLine("[ERREUR] Pas assez de ressources !");
                    }
                }
            }
            else
            {
                Console.WriteLine("ID introuvable.");
            }
        }
    }

    static void AfficherEtatGlobal()
    {
        Console.WriteLine("\n=== RAPPORT ===");
        Console.WriteLine($"[POPULATION] {_population.GetStatusString()}");
        Console.WriteLine("[RESSOURCES]");
        foreach (ResourceTypeEnums type in Enum.GetValues(typeof(ResourceTypeEnums)))
        {
            int stock = _ressources.GetStock(type);
            int max = _ressources.GetMax(type);
            Console.WriteLine($"- {type,-12} : {stock} / {max}");
        }
    }

    static void AfficherBatiments()
    {
        Console.WriteLine("\n=== VOS BÂTIMENTS ===");
        foreach (var b in _batiments)
        {
            string details = "";
            if (b is ProductionBuilding prod)
                details = $"-> Prod: {prod.TauxProduction} {prod.ResourceProduite}/sec";
            else if (b is HousingBuilding house)
                details = $"-> Lits: {house.CapaciteHabitants}";
            else if (b is StorageBuilding storage)
                details = $"-> Stockage: +{storage.CapaciteAjoutee}";

            Console.WriteLine($"[ID {b.Id}] {b.Nom} (Niv {b.Level}) {details}");
        }
    }

    static void ActionConstruireBatiment()
    {
       
        Console.WriteLine("\n=== MENU CONSTRUCTION ===");
        Console.WriteLine("1. Mine de Fer (50 Fer)");
        Console.WriteLine("2. Mine d'Or (150 Fer)");
        Console.WriteLine("3. Ferme (30 Fer)");
        Console.WriteLine("4. Maison (50 Fer)");
        Console.WriteLine("5. Silo Patates (80 Fer)");
        Console.Write("> ");

        var choix = Console.ReadKey(true).Key;
        int newId = (_batiments.Count > 0 ? _batiments.Max(b => b.Id) : 0) + 1;

        if (choix == ConsoleKey.D1) TryBuild(50, new Mine(newId, ResourceTypeEnums.Fer));
        else if (choix == ConsoleKey.D2) TryBuild(150, new Mine(newId, ResourceTypeEnums.Or));
        else if (choix == ConsoleKey.D3) TryBuild(30, new Ferme(newId));
        else if (choix == ConsoleKey.D4)
        {
            if (TryBuild(50, new HousingBuilding(newId))) _population.UpdateMaxPopulation(_batiments);
        }
        else if (choix == ConsoleKey.D5)
        {
            if (TryBuild(80, new StorageBuilding(newId, ResourceTypeEnums.Patate))) _ressources.UpdateMaxStorage(_batiments);
        }
    }

    static bool TryBuild(int cost, Building b)
    {
        if (_ressources.HasEnough(ResourceTypeEnums.Fer, cost))
        {
            _ressources.Retirer(ResourceTypeEnums.Fer, cost);
            _batiments.Add(b);
            Console.WriteLine($"\n[SUCCÈS] Construit : {b.Nom}");
            return true;
        }
        Console.WriteLine("\n[ERREUR] Pas assez de Fer.");
        return false;
    }

    static void ActionRetirerRessource() { /* ...  }
    static void ActionAjouterPopulation() { _population.Ajouter(1); Console.WriteLine("+1"); }
    static void ActionRetirerPopulation() { _population.Retirer(1); Console.WriteLine("-1"); }
    static void PauseUtilisateur() { Console.WriteLine("\n[Entrée...]"); Console.ReadLine(); }

    static async Task BoucleDeJeu()
    {
        while (_jeuEnCours)
        {
            _population.UpdateMaxPopulation(_batiments);
            _ressources.UpdateMaxStorage(_batiments);

            try
            {
                foreach (var b in _batiments.ToList())
                {
                    if (b is ProductionBuilding p) p.Produire(_ressources);
                }
            }
            catch { }

            int nb = _population.GetStock();
            if (nb > 0)
            {
                if (_ressources.HasEnough(ResourceTypeEnums.Patate, nb)) _ressources.Retirer(ResourceTypeEnums.Patate, nb);
                else _population.Retirer(1);
            }
            await Task.Delay(1000);
        }
    }
}

*/

