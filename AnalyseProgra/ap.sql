PRAGMA foreign_keys = OFF;

DROP TABLE IF EXISTS game_saves;
DROP TABLE IF EXISTS moderation_actions;
DROP TABLE IF EXISTS colony_population;
DROP TABLE IF EXISTS colony_buildings;
DROP TABLE IF EXISTS building_type_costs;
DROP TABLE IF EXISTS building_types;
DROP TABLE IF EXISTS colony_resources;
DROP TABLE IF EXISTS resource_types;
DROP TABLE IF EXISTS colonies;
DROP TABLE IF EXISTS users;
DROP TABLE IF EXISTS roles;

PRAGMA foreign_keys = ON;

CREATE TABLE roles
(
    id   INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE users
(
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    username      TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role_id       INTEGER NOT NULL,
    is_active     INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (role_id) REFERENCES roles (id)
);

CREATE TABLE colonies
(
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id    INTEGER NOT NULL UNIQUE,
    name       TEXT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users (id)
);

CREATE TABLE resource_types
(
    id   INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE colony_resources
(
    colony_id        INTEGER NOT NULL,
    resource_type_id INTEGER NOT NULL,
    quantity         REAL NOT NULL DEFAULT 0,
    production_rate  REAL NOT NULL DEFAULT 0,
    consumption_rate REAL NOT NULL DEFAULT 0,
    PRIMARY KEY (colony_id, resource_type_id),
    FOREIGN KEY (colony_id) REFERENCES colonies (id) ON DELETE CASCADE,
    FOREIGN KEY (resource_type_id) REFERENCES resource_types (id)
);

CREATE TABLE building_types
(
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL UNIQUE,
    description TEXT
);

CREATE TABLE building_type_costs
(
    building_type_id INTEGER NOT NULL,
    resource_type_id INTEGER NOT NULL,
    amount           REAL NOT NULL,
    PRIMARY KEY (building_type_id, resource_type_id),
    FOREIGN KEY (building_type_id) REFERENCES building_types (id) ON DELETE CASCADE,
    FOREIGN KEY (resource_type_id) REFERENCES resource_types (id)
);

CREATE TABLE colony_buildings
(
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    colony_id        INTEGER NOT NULL,
    building_type_id INTEGER NOT NULL,
    level            INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (colony_id) REFERENCES colonies (id) ON DELETE CASCADE,
    FOREIGN KEY (building_type_id) REFERENCES building_types (id)
);

CREATE TABLE colony_population
(
    colony_id        INTEGER PRIMARY KEY,
    population_count INTEGER NOT NULL DEFAULT 0,
    morale           REAL NOT NULL DEFAULT 100,
    FOREIGN KEY (colony_id) REFERENCES colonies (id) ON DELETE CASCADE
);

CREATE TABLE moderation_actions
(
    id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    performed_by_user_id INTEGER NOT NULL,
    target_user_id       INTEGER NOT NULL,
    action_type          TEXT NOT NULL,
    details              TEXT,
    FOREIGN KEY (performed_by_user_id) REFERENCES users (id),
    FOREIGN KEY (target_user_id) REFERENCES users (id)
);

CREATE TABLE game_saves
(
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id    INTEGER NOT NULL,
    colony_id  INTEGER NOT NULL,
    save_name  TEXT NOT NULL,
    data_json  TEXT NOT NULL,
    FOREIGN KEY (user_id)   REFERENCES users (id),
    FOREIGN KEY (colony_id) REFERENCES colonies (id) ON DELETE CASCADE
);
