using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using AnalyseProgra.DataAccess.Interface;

namespace AnalyseProgra.DataAccess.Dao
{
    public class UserDao : IUserDao
    {
        public User? GetById(int id, bool includeDetails = false)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, username, password_hash, role, is_active
                FROM users
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            var user = reader.Read() ? Map(reader) : null;

            if (user == null || !includeDetails)
                return user;

            LoadRelations(user);
            return user;
        }

        public User? GetByUsername(string username, bool includeDetails = false)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, username, password_hash, role, is_active
                FROM users
                WHERE username = $u;
            ";
            cmd.Parameters.AddWithValue("$u", username);

            using var reader = cmd.ExecuteReader();
            var user = reader.Read() ? Map(reader) : null;

            if (user == null || !includeDetails)
                return user;

            LoadRelations(user);
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var list = new List<User>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, username, password_hash, role, is_active
                FROM users;
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));

            return list;
        }

        public User Create(User user)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO users (username, password_hash, role, is_active)
                VALUES ($u, $p, $r, $a);

                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$u", user.Username);
            cmd.Parameters.AddWithValue("$p", user.PasswordHash);
            cmd.Parameters.AddWithValue("$r", (int)user.Role);
            cmd.Parameters.AddWithValue("$a", user.IsActive ? 1 : 0);

            var newId = (long)cmd.ExecuteScalar();
            user.Id = (int)newId;

            return user;
        }

        public void Update(User user)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE users
                SET username = $u,
                    password_hash = $p,
                    role = $r,
                    is_active = $a
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$u", user.Username);
            cmd.Parameters.AddWithValue("$p", user.PasswordHash);
            cmd.Parameters.AddWithValue("$r", (int)user.Role);
            cmd.Parameters.AddWithValue("$a", user.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", user.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM users WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static User Map(SqliteDataReader r)
        {
            return new User
            {
                Id = r.GetInt32(0),
                Username = r.GetString(1),
                PasswordHash = r.GetString(2),
                Role = (UserRole)r.GetInt32(3),
                IsActive = r.GetInt32(4) != 0,
                Colonies = new List<Colony>()
            };
        }

        private static void LoadRelations(User user)
        {
            var colonyDao = new ColonyDao();
            user.Colonies = colonyDao.GetByOwner(user.Username, includeDetails: true).ToList();
        }
    }
}
