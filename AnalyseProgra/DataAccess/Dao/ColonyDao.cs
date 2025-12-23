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
    public class ColonyDao : IColonyDao
    {
        public Colony? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, name
                FROM colonies
                WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public Colony? GetByUserId(int userId)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, name
                FROM colonies
                WHERE user_id = $uid;";
            cmd.Parameters.AddWithValue("$uid", userId);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<Colony> GetAll()
        {
            var list = new List<Colony>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, user_id, name FROM colonies;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public Colony Create(Colony colony)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO colonies (user_id, name)
                VALUES ($uid, $name);

                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$uid", colony.UserId);
            cmd.Parameters.AddWithValue("$name", colony.Name);

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
                UPDATE colonies
                SET user_id = $uid,
                    name = $name
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$uid", colony.UserId);
            cmd.Parameters.AddWithValue("$name", colony.Name);
            cmd.Parameters.AddWithValue("$id", colony.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM colonies WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static Colony Map(SqliteDataReader r)
        {
            return new Colony
            {
                Id = r.GetInt32(0),
                UserId = r.GetInt32(1),
                Name = r.GetString(2)
            };
        }
    }
}
