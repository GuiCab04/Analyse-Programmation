using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;

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
                SELECT colony_id, resource_name, quantity
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

        public ColonyResource? GetOne(int colonyId, string resourceName)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, resource_name, quantity
                FROM colony_resource
                WHERE colony_id = $c AND resource_name = $r;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$r", resourceName);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public ColonyResource Create(ColonyResource resource)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_resource (colony_id, resource_name, quantity)
                VALUES ($c, $r, $q);
            ";
            cmd.Parameters.AddWithValue("$c", resource.ColonyId);
            cmd.Parameters.AddWithValue("$r", resource.ResourceName);
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
                WHERE colony_id = $c AND resource_name = $r;
            ";
            cmd.Parameters.AddWithValue("$q", resource.Quantity);
            cmd.Parameters.AddWithValue("$c", resource.ColonyId);
            cmd.Parameters.AddWithValue("$r", resource.ResourceName);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId, string resourceName)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_resource
                WHERE colony_id = $c AND resource_name = $r;
            ";
            cmd.Parameters.AddWithValue("$c", colonyId);
            cmd.Parameters.AddWithValue("$r", resourceName);

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
            return new ColonyResource
            {
                ColonyId = r.GetInt32(0),
                ResourceName = r.GetString(1),
                Quantity = r.GetDouble(2)
            };
        }
    }
}
