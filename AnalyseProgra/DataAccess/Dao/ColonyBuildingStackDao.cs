using System.Collections.Generic;
using System.Reflection.Emit;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;
using Microsoft.Data.Sqlite;
using AnalyseProgra.Models.Buildings;
using AnalyseProgra.Core.Managers;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyBuildingStackDao : IColonyBuildingStackDao
    {
        public IEnumerable<ColonyBuildingStack> GetByColonyId(int colonyId)
        {
            var list = new List<ColonyBuildingStack>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, building_type, level, amount
                FROM colony_building_stack
                WHERE colony_id = $c;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public ColonyBuildingStack? GetOne(int colonyId, BuildingType buildingType, int level)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, building_type, level, amount
                FROM colony_building_stack
                WHERE colony_id = $c AND building_type = $b AND level = $l;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$b", (int)buildingType);
            cmd.Parameters.AddWithValue("$l", level);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public ColonyBuildingStack Create(ColonyBuildingStack stack)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_building_stack (colony_id, building_type, level, amount)
                VALUES ($c, $b, $l, $a);
            ";
            cmd.Parameters.AddWithValue("$c", stack.ColonyId);
            cmd.Parameters.AddWithValue("$b", (int)stack.BuildingType);
            cmd.Parameters.AddWithValue("$l", stack.Level);
            cmd.Parameters.AddWithValue("$a", stack.Amount);

            cmd.ExecuteNonQuery();
            return stack;
        }

        public void Update(ColonyBuildingStack stack)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony_building_stack
                SET amount = $a
                WHERE colony_id = $c AND building_type = $b AND level = $l;
            ";
            cmd.Parameters.AddWithValue("$a", stack.Amount);
            cmd.Parameters.AddWithValue("$c", stack.ColonyId);
            cmd.Parameters.AddWithValue("$b", (int)stack.BuildingType);
            cmd.Parameters.AddWithValue("$l", stack.Level);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId, BuildingType buildingType, int level)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_building_stack
                WHERE colony_id = $c AND building_type = $b AND level = $l;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$b", (int)buildingType);
            cmd.Parameters.AddWithValue("$l", level);

            cmd.ExecuteNonQuery();
        }

        public void DeleteByColony(int colonyId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_building_stack
                WHERE colony_id = $c;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);

            cmd.ExecuteNonQuery();
        }

        private static ColonyBuildingStack Map(SqliteDataReader r)
        {
            var buildingType = (BuildingType)r.GetInt32(1);
            var buildingLevel = r.GetInt32(2);
            ColonyBuildingStack newColonyBuildingStack = BuildingManager.BuildBuildingStack(buildingType, buildingLevel);

            newColonyBuildingStack.ColonyId = r.GetInt32(0);
            newColonyBuildingStack.BuildingType = buildingType;
            newColonyBuildingStack.Amount = r.GetInt32(3);
            return newColonyBuildingStack;
        }
    }
}
