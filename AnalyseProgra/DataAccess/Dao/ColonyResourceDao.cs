using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyResourceDao : IColonyResourceDao
    {
        public ColonyResource? Get(int colonyId, int resourceTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, resource_type_id, quantity, production_rate, consumption_rate
                FROM colony_resources
                WHERE colony_id = $cid AND resource_type_id = $rid;
            ";
            cmd.Parameters.AddWithValue("$cid", colonyId);
            cmd.Parameters.AddWithValue("$rid", resourceTypeId);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<ColonyResource> GetByColony(int colonyId)
        {
            var list = new List<ColonyResource>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, resource_type_id, quantity, production_rate, consumption_rate
                FROM colony_resources
                WHERE colony_id = $cid;
            ";
            cmd.Parameters.AddWithValue("$cid", colonyId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public ColonyResource Create(ColonyResource cr)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_resources (colony_id, resource_type_id, quantity, production_rate, consumption_rate)
                VALUES ($cid, $rid, $q, $prod, $cons);
            ";

            cmd.Parameters.AddWithValue("$cid", cr.ColonyId);
            cmd.Parameters.AddWithValue("$rid", cr.ResourceTypeId);
            cmd.Parameters.AddWithValue("$q", cr.Quantity);
            cmd.Parameters.AddWithValue("$prod", cr.ProductionRate);
            cmd.Parameters.AddWithValue("$cons", cr.ConsumptionRate);

            cmd.ExecuteNonQuery();
            return cr;
        }

        public void Update(ColonyResource cr)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony_resources
                SET quantity = $q,
                    production_rate = $prod,
                    consumption_rate = $cons
                WHERE colony_id = $cid AND resource_type_id = $rid;
            ";

            cmd.Parameters.AddWithValue("$cid", cr.ColonyId);
            cmd.Parameters.AddWithValue("$rid", cr.ResourceTypeId);
            cmd.Parameters.AddWithValue("$q", cr.Quantity);
            cmd.Parameters.AddWithValue("$prod", cr.ProductionRate);
            cmd.Parameters.AddWithValue("$cons", cr.ConsumptionRate);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId, int resourceTypeId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM colony_resources
                WHERE colony_id = $cid AND resource_type_id = $rid;
            ";
            cmd.Parameters.AddWithValue("$cid", colonyId);
            cmd.Parameters.AddWithValue("$rid", resourceTypeId);

            cmd.ExecuteNonQuery();
        }

        private static ColonyResource Map(SqliteDataReader r)
        {
            return new ColonyResource
            {
                ColonyId = r.GetInt32(0),
                ResourceTypeId = r.GetInt32(1),
                Quantity = r.GetDouble(2),
                ProductionRate = r.GetDouble(3),
                ConsumptionRate = r.GetDouble(4)
            };
        }
    }
}

