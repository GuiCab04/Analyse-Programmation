using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ColonyPopulationDao : IColonyPopulationDao
    {
        public ColonyPopulation? Get(int colonyId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT colony_id, population_count, morale
                FROM colony_population
                WHERE colony_id = $cid;
            ";
            cmd.Parameters.AddWithValue("$cid", colonyId);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public ColonyPopulation Create(ColonyPopulation pop)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colony_population (colony_id, population_count, morale)
                VALUES ($cid, $pop, $morale);
            ";

            cmd.Parameters.AddWithValue("$cid", pop.ColonyId);
            cmd.Parameters.AddWithValue("$pop", pop.PopulationCount);
            cmd.Parameters.AddWithValue("$morale", pop.Morale);

            cmd.ExecuteNonQuery();
            return pop;
        }

        public void Update(ColonyPopulation pop)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE colony_population
                SET population_count = $pop,
                    morale = $morale
                WHERE colony_id = $cid;
            ";

            cmd.Parameters.AddWithValue("$cid", pop.ColonyId);
            cmd.Parameters.AddWithValue("$pop", pop.PopulationCount);
            cmd.Parameters.AddWithValue("$morale", pop.Morale);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int colonyId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM colony_population WHERE colony_id = $cid;";
            cmd.Parameters.AddWithValue("$cid", colonyId);

            cmd.ExecuteNonQuery();
        }

        private static ColonyPopulation Map(SqliteDataReader r)
        {
            return new ColonyPopulation
            {
                ColonyId = r.GetInt32(0),
                PopulationCount = r.GetInt32(1),
                Morale = r.GetDouble(2)
            };
        }
    }
}
