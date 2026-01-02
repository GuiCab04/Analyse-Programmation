using System.Collections.Generic;
using System.Linq;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyDao : IColonyDao
    {
        public Colony? GetById(int id, bool includeDetails = false)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, owner_username, name, population_count, morale
                FROM colony
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            var colony = Map(reader);

            if (includeDetails)
                LoadDetails(colony);

            return colony;
        }

        public IEnumerable<Colony> GetByOwner(string username, bool includeDetails = false)
        {
            var colonies = new List<Colony>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, owner_username, name, population_count, morale
                FROM colony
                WHERE owner_username = $u;
            ";
            cmd.Parameters.AddWithValue("$u", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                colonies.Add(Map(reader));

            if (!includeDetails)
                return colonies;

            foreach (var c in colonies)
                LoadDetails(c);

            return colonies;
        }

        public IEnumerable<Colony> GetAll(bool includeDetails = false)
        {
            var list = new List<Colony>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, owner_username, name, population_count, morale
                FROM colony;
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));

            if (!includeDetails)
                return list;

            foreach (var colony in list)
                LoadDetails(colony);

            return list;
        }

        public Colony Create(Colony colony)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony (owner_username, name, population_count, morale)
                VALUES ($o, $n, $p, $m);

                SELECT last_insert_rowid();
            ";
            cmd.Parameters.AddWithValue("$o", colony.OwnerUsername);
            cmd.Parameters.AddWithValue("$n", colony.Name);
            cmd.Parameters.AddWithValue("$p", colony.Population.CurrentPopulation);
            cmd.Parameters.AddWithValue("$m", colony.Morale);

            var newId = (long)cmd.ExecuteScalar();
            colony.Id = (int)newId;

            return colony;
        }

        public void Update(Colony colony)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony
                SET owner_username = $o,
                    name = $n,
                    population_count = $p,
                    morale = $m
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$o", colony.OwnerUsername);
            cmd.Parameters.AddWithValue("$n", colony.Name);
            cmd.Parameters.AddWithValue("$p", colony.Population.CurrentPopulation);
            cmd.Parameters.AddWithValue("$m", colony.Morale);
            cmd.Parameters.AddWithValue("$id", colony.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var tx = conn.BeginTransaction();

            var stackDao = new ColonyBuildingStackDao();
            var resDao = new ColonyResourceDao();

            stackDao.DeleteByColony(id);
            resDao.DeleteByColony(id);

            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM colony WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        private static Colony Map(SqliteDataReader r)
        {
            var colony = new Colony
            {
                Id = r.GetInt32(0),
                OwnerUsername = r.GetString(1),
                Name = r.GetString(2),
                Morale = r.GetDouble(4)
            };
            colony.Population.CurrentPopulation = r.GetInt32(3);

            return colony;
        }

        private static void LoadDetails(Colony colony)
        {
            var stackDao = new ColonyBuildingStackDao();
            var resDao = new ColonyResourceDao();

            colony.Buildings.BuildingStacks = new List<ColonyBuildingStack>(stackDao.GetByColonyId(colony.Id));
            colony.Resources.Resources = new List<ColonyResource>(resDao.GetByColonyId(colony.Id));
        }

        public void SaveWithRelations(Colony colony)
        {
            var buildingStackDao = new ColonyBuildingStackDao();
            var resourceDao = new ColonyResourceDao();
            
            if (colony.Id == 0)
            {
                Create(colony);
            }
            else
            {
                Update(colony);
            }

            var existingResources = resourceDao.GetByColonyId(colony.Id);
            foreach (var resource in colony.Resources.Resources)
            {
                bool resourceExists = existingResources.Any(r => r.ResourceType == resource.ResourceType);
                if (!resourceExists)
                {
                    resource.ColonyId = colony.Id;
                    resourceDao.Create(resource);
                }
                else
                {
                    resourceDao.Update(resource);
                }
            }

            var existingStacks = buildingStackDao.GetByColonyId(colony.Id);
            foreach (var stack in colony.Buildings.BuildingStacks)
            {
                bool stackExists = existingStacks.Any(s => s.BuildingType == stack.BuildingType && s.Level == stack.Level);
                if (!stackExists)
                {
                    stack.ColonyId = colony.Id;
                    buildingStackDao.Create(stack);
                }
                else
                {
                    buildingStackDao.Update(stack);
                }
            }
        }
    }
}
