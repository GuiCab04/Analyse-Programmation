using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyBuildingDao : IColonyBuildingDao
    {
        public ColonyBuilding? GetByColonyAndType(int colonyId, string buildingTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, building_type_id, level
                FROM colony_buildings
                WHERE colony_id = $c AND building_type_id = $t;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$t", buildingTypeId);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<ColonyBuilding> GetByColony(int colonyId)
        {
            var list = new List<ColonyBuilding>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, building_type_id, level
                FROM colony_buildings
                WHERE colony_id = $c;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));

            return list;
        }

        public ColonyBuilding Create(ColonyBuilding building)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_buildings (colony_id, building_type_id, level)
                VALUES ($c, $t, $l);
            ";

            cmd.Parameters.AddWithValue("$c", building.ColonyId);
            cmd.Parameters.AddWithValue("$t", building.BuildingTypeId);
            cmd.Parameters.AddWithValue("$l", building.Level);

            cmd.ExecuteNonQuery();
            return building;
        }

        public void Update(ColonyBuilding building)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony_buildings
                SET level = $l
                WHERE colony_id = $c AND building_type_id = $t;
            ";

            cmd.Parameters.AddWithValue("$l", building.Level);
            cmd.Parameters.AddWithValue("$c", building.ColonyId);
            cmd.Parameters.AddWithValue("$t", building.BuildingTypeId);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId, string buildingTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_buildings
                WHERE colony_id = $c AND building_type_id = $t;
            ";

            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$t", buildingTypeId);

            cmd.ExecuteNonQuery();
        }

        private static ColonyBuilding Map(SqliteDataReader r)
        {
            return new ColonyBuilding
            {
                ColonyId = r.GetInt32(0),
                BuildingTypeId = r.GetString(1),
                Level = r.GetInt32(2)
            };
        }
    }
}
