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
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE resource_types (
  name TEXT PRIMARY KEY
);

CREATE TABLE building_types (
  name TEXT PRIMARY KEY,
  description TEXT
);

CREATE TABLE colony_resources (
  colony_id        INTEGER NOT NULL,
  resource_type_id TEXT NOT NULL,
  quantity         REAL NOT NULL DEFAULT 0,
  production_rate  REAL NOT NULL DEFAULT 0,
  consumption_rate REAL NOT NULL DEFAULT 0,
  PRIMARY KEY (colony_id, resource_type_id),
  FOREIGN KEY (colony_id) REFERENCES colonies(id) ON DELETE CASCADE,
  FOREIGN KEY (resource_type_id) REFERENCES resource_types(name)
);

CREATE TABLE building_type_costs (
  building_type_id TEXT NOT NULL,
  resource_type_id TEXT NOT NULL,
  amount           REAL NOT NULL,
  PRIMARY KEY (building_type_id, resource_type_id),
  FOREIGN KEY (building_type_id) REFERENCES building_types(name) ON DELETE CASCADE,
  FOREIGN KEY (resource_type_id) REFERENCES resource_types(name)
);

CREATE TABLE colony_buildings (
  colony_id        INTEGER NOT NULL,
  building_type_id TEXT NOT NULL,
  level            INTEGER NOT NULL DEFAULT 1,
  PRIMARY KEY (colony_id, building_type_id),
  FOREIGN KEY (colony_id) REFERENCES colonies(id) ON DELETE CASCADE,
  FOREIGN KEY (building_type_id) REFERENCES building_types(name)
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
