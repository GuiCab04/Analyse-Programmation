PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS colony_resource;
DROP TABLE IF EXISTS colony_building_stack;
DROP TABLE IF EXISTS colony;
DROP TABLE IF EXISTS users;

CREATE TABLE IF NOT EXISTS users (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    username      TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role          INTEGER NOT NULL,
    is_active     INTEGER NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS colony (
    id               INTEGER PRIMARY KEY AUTOINCREMENT,
    owner_username   TEXT NOT NULL UNIQUE,
    name             TEXT NOT NULL,
    population_count INTEGER NOT NULL DEFAULT 0,
    morale           REAL    NOT NULL DEFAULT 100,
    FOREIGN KEY (owner_username) REFerENCES users(username) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS colony_building_stack (
    colony_id      INTEGER NOT NULL,
    building_type  INTEGER NOT NULL,
    level          INTEGER NOT NULL,
    amount         INTEGER NOT NULL DEFAULT 1,
    PRIMARY KEY (colony_id, building_type, level),
    FOREIGN KEY (colony_id) REFerENCES colony(id) ON DELETE CASCADE,
    CHECK (building_type IN (0,1,2,3,4,5,6,7,8,9,10,11))
);

CREATE TABLE IF NOT EXISTS colony_resource (
    colony_id        INTEGER NOT NULL,
    resource_type    INTEGER    NOT NULL,
    quantity         INTEGER    NOT NULL DEFAULT 0,
    PRIMARY KEY (colony_id, resource_type),
    FOREIGN KEY (colony_id) REFerENCES colony(id) ON DELETE CASCADE
);
