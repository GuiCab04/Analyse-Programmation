using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models.Enums;
using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Security.AccessControl;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyResourceDao : IColonyResourceDao
    {
        public IEnumerable<ColonyResource> GetByColonyId(int colonyId)
        {
            var list = new List<ColonyResource>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, resource_type, quantity
                FROM colony_resource
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

        public ColonyResource? GetOne(int colonyId, ResourceTypeEnums resourceType)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, resource_type, quantity
                FROM colony_resource
                WHERE colony_id = $c AND resource_type = $r;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$r", resourceType);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public ColonyResource Create(ColonyResource resource)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_resource (colony_id, resource_type, quantity)
                VALUES ($c, $r, $q);
            ";
            cmd.Parameters.AddWithValue("$c", resource.ColonyId);
            cmd.Parameters.AddWithValue("$r", resource.ResourceType);
            cmd.Parameters.AddWithValue("$q", resource.Quantity);

            cmd.ExecuteNonQuery();
            return resource;
        }

        public void Update(ColonyResource resource)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony_resource
                SET quantity = $q
                WHERE colony_id = $c AND resource_type = $r;
            ";
            cmd.Parameters.AddWithValue("$q", resource.Quantity);
            cmd.Parameters.AddWithValue("$c", resource.ColonyId);
            cmd.Parameters.AddWithValue("$r", resource.ResourceType);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId, ResourceTypeEnums resourceType)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_resource
                WHERE colony_id = $c AND resource_type = $r;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$r", resourceType);

            cmd.ExecuteNonQuery();
        }

        public void DeleteByColony(int colonyId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_resource
                WHERE colony_id = $c;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);

            cmd.ExecuteNonQuery();
        }

        private static ColonyResource Map(SqliteDataReader r)
        {
            var resourceType = (ResourceTypeEnums)r.GetInt32(1);
            var quantity = r.GetDouble(2);

            return new ColonyResource(resourceType, quantity)
            {
                ColonyId = r.GetInt32(0)
            };
        }
    }
}
