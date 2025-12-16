using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class BuildingTypeCostDao : IBuildingTypeCostDao
    {
        public BuildingTypeCost? Get(int buildingTypeId, int resourceTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT building_type_id, resource_type_id, amount
                FROM building_type_costs
                WHERE building_type_id = $b AND resource_type_id = $r;
            ";

            cmd.Parameters.AddWithValue("$b", buildingTypeId);
            cmd.Parameters.AddWithValue("$r", resourceTypeId);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<BuildingTypeCost> GetByBuildingType(int buildingTypeId)
        {
            var list = new List<BuildingTypeCost>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT building_type_id, resource_type_id, amount
                FROM building_type_costs
                WHERE building_type_id = $b;
            ";

            cmd.Parameters.AddWithValue("$b", buildingTypeId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public BuildingTypeCost Create(BuildingTypeCost cost)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO building_type_costs (building_type_id, resource_type_id, amount)
                VALUES ($b, $r, $a);
            ";

            cmd.Parameters.AddWithValue("$b", cost.BuildingTypeId);
            cmd.Parameters.AddWithValue("$r", cost.ResourceTypeId);
            cmd.Parameters.AddWithValue("$a", cost.Amount);

            cmd.ExecuteNonQuery();
            return cost;
        }

        public void Update(BuildingTypeCost cost)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE building_type_costs
                SET amount = $a
                WHERE building_type_id = $b AND resource_type_id = $r;
            ";

            cmd.Parameters.AddWithValue("$a", cost.Amount);
            cmd.Parameters.AddWithValue("$b", cost.BuildingTypeId);
            cmd.Parameters.AddWithValue("$r", cost.ResourceTypeId);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int buildingTypeId, int resourceTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM building_type_costs
                WHERE building_type_id = $b AND resource_type_id = $r;
            ";

            cmd.Parameters.AddWithValue("$b", buildingTypeId);
            cmd.Parameters.AddWithValue("$r", resourceTypeId);

            cmd.ExecuteNonQuery();
        }

        private static BuildingTypeCost Map(SqliteDataReader r)
        {
            return new BuildingTypeCost
            {
                BuildingTypeId = r.GetInt32(0),
                ResourceTypeId = r.GetInt32(1),
                Amount = r.GetDouble(2)
            };
        }
    }
}
