using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class RoleDao : IRoleDao
    {
        public Role? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, name
                FROM roles
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public Role? GetByName(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, name
                FROM roles
                WHERE name = $name;
            ";
            cmd.Parameters.AddWithValue("$name", name);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<Role> GetAll()
        {
            var list = new List<Role>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, name
                FROM roles;
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public Role Create(Role role)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO roles (name)
                VALUES ($name);

                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$name", role.Name);

            var newId = (long)cmd.ExecuteScalar();
            role.Id = (int)newId;

            return role;
        }

        public void Update(Role role)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE roles
                SET name = $name
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$name", role.Name);
            cmd.Parameters.AddWithValue("$id", role.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM roles WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static Role Map(SqliteDataReader r)
        {
            return new Role
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            };
        }
    }
}
