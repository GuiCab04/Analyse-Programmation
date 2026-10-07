# Analyse Progra - Simulation & Gestion de Colonie

Application console de gestion et de simulation de colonie spatiale développée en **C# (.NET 7)** dans le cadre du cours d'**Analyse et Programmation** (Master 1 Complément Informatique — Hénallux).

---

## 🌟 Fonctionnalités

### 🏛️ Gestion de la Colonie
- **Bâtiments constructibles et améliorables :**
  - **Mairie :** Centre administratif de la colonie.
  - **Habitations :** Augmentent la capacité maximale de population.
  - **Fermes & Fermes Sali :** Production de ressources nutritives (Solurial, Sali).
  - **Mines :** Extraction minière de ressources (Soluro, Soli, Solu).
  - **Entrepôts de stockage :** Augmentent la limite de stockage pour chaque type de ressource.
- **Ressources :** Solurial, Sali, Soluro, Soli, Solu.
- **Population & Moral :**
  - Arrivée dynamique de nouveaux colons conditionnée par le moral et les réserves de Solurial.
  - Consommation périodique de nourriture par les colons.
  - Gestion du moral (baisse en cas de pénurie, hausse en cas de prospérité).

### ⚙️ Moteur de Jeu Asynchrone
- Boucle de jeu en arrière-plan simulant en temps réel :
  - La production continue des bâtiments de production.
  - La consommation de nourriture par la population.
  - L'évolution dynamique du moral et des effectifs.

### 👥 Système d'Authentification & Rôles
- **Administrateur :**
  - Gestion des utilisateurs (activation/désactivation de comptes).
  - Attribution des rôles (nomination de modérateurs).
- **Modérateur :**
  - Accès au panneau de modération.
- **Joueur :**
  - Accès au tableau de bord complet de gestion de la colonie.
  - Achat, amélioration et suppression de bâtiments.
  - Suivi de la population et des stocks en temps réel.

### 🖥️ Interface Utilisateur Riche
- Interface console moderne et interactive propulsée par **Spectre.Console** (tableaux de bord stylisés, invites de sélection interactives, messages colorés).

### 💾 Persistance des Données
- Base de données relationnelle **SQLite** (`game.db`).
- Architecture organisée avec des DAO (*Data Access Objects*) :
  - `UserDao`
  - `ColonyDao`
  - `ColonyBuildingStackDao`
  - `ColonyResourceDao`
- Script SQL d'initialisation : `ap.sql`.

---

## 🛠️ Stack Technique

- **Langage :** C# (.NET 7.0)
- **Interface Console :** [Spectre.Console](https://spectreconsole.net/) (v0.54.0)
- **Base de données :** SQLite avec [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite) (v10.0.0)

---

## 📁 Structure du Projet

```text
Analyse-Programmation/
├── AnalyseProgra.sln
├── AnalyseProgra/
│   ├── Core/
│   │   └── Managers/           # Logique métier (BuildingManager, PopulationManager, ResourceManager)
│   ├── DataAccess/
│   │   ├── Dao/                # Implémentations DAO SQLite
│   │   ├── Interface/          # Interfaces DAO
│   │   └── Db.cs               # Connexion à la base de données
│   ├── Interactions/           # Actions utilisateur (Login, Achat, Amélioration, Sauvegarde, Admin...)
│   ├── Models/
│   │   ├── Buildings/          # Modèles de bâtiments (Ferme, Mine, Mairie, Logement, Stockage)
│   │   ├── Enums/              # Énumérations (BuildingType, ResourceType, UserRole)
│   │   └── Users/              # Modèles Utilisateurs (Admin, Modérateur, Joueur)
│   ├── Views/                  # Interface console et écrans Spectre.Console
│   ├── ap.sql                  # Schéma de la base de données
│   ├── game.db                 # Base SQLite active
│   ├── Program.cs              # Point d'entrée de l'application
│   └── AnalyseProgra.csproj
├── .gitattributes
├── .gitignore
└── README.md
```

---

## 🚀 Installation & Lancement

### Prérequis
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) ou supérieur.

### Démarrage rapide

1. **Cloner le dépôt :**
   ```bash
   git clone https://github.com/GuiCab04/Analyse-Programmation.git
   cd Analyse-Programmation
   ```

2. **Restaurer les dépendances et compiler :**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Exécuter le projet :**
   ```bash
   dotnet run --project AnalyseProgra
   ```

### Initialisation de la base de données (si nécessaire)
Le fichier `game.db` est fourni préconfiguré. Si vous souhaitez réinitialiser la base de données depuis le schéma `ap.sql` :
```bash
cd AnalyseProgra
.\sqlite3.exe game.db < ap.sql
```

### Compte Administrateur par défaut
Si la base est vide lors du premier lancement, un compte administrateur est automatiquement généré :
- **Identifiant :** `admin`
- **Mot de passe :** `admin`

---

## 👥 Auteurs
- Guillaume Cabaraux ([@GuiCab04](https://github.com/GuiCab04))
- Étudiants du groupe B — Henallux MASI (2025-2026)
