using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
	public class UserDao : IUserDao
	{
		public User? GetById(int id)
		{
			using var conn = Db.GetConnection();
			conn.Open();

			using var cmd = conn.CreateCommand();
			cmd.CommandText = @"
                SELECT id, username, password_hash, role_id, is_active
                FROM users
                WHERE id = $id;
            ";
			cmd.Parameters.AddWithValue("$id", id);

			using var reader = cmd.ExecuteReader();
			return reader.Read() ? Map(reader) : null;
		}

		public User? GetByUsername(string username)
		{
			using var conn = Db.GetConnection();
			conn.Open();

			using var cmd = conn.CreateCommand();
			cmd.CommandText = @"
                SELECT id, username, password_hash, role_id, is_active
                FROM users
                WHERE username = $u;
            ";
			cmd.Parameters.AddWithValue("$u", username);

			using var reader = cmd.ExecuteReader();
			return reader.Read() ? Map(reader) : null;
		}

		public IEnumerable<User> GetAll()
		{
			var list = new List<User>();

			using var conn = Db.GetConnection();
			conn.Open();

			using var cmd = conn.CreateCommand();
			cmd.CommandText = @"
                SELECT id, username, password_hash, role_id, is_active
                FROM users;
            ";

			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				list.Add(Map(reader));
			}

			return list;
		}

		public User Create(User user)
		{
			using var conn = Db.GetConnection();
			conn.Open();

			using var cmd = conn.CreateCommand();
			cmd.CommandText = @"
                INSERT INTO users (username, password_hash, role_id, is_active)
                VALUES ($u, $p, $r, $a);

                SELECT last_insert_rowid();
            ";

			cmd.Parameters.AddWithValue("$u", user.Username);
			cmd.Parameters.AddWithValue("$p", user.PasswordHash);
			cmd.Parameters.AddWithValue("$r", user.RoleId);
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
                    role_id = $r,
                    is_active = $a
                WHERE id = $id;
            ";

			cmd.Parameters.AddWithValue("$u", user.Username);
			cmd.Parameters.AddWithValue("$p", user.PasswordHash);
			cmd.Parameters.AddWithValue("$r", user.RoleId);
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
				RoleId = r.GetInt32(3),
				IsActive = r.GetInt32(4) != 0
			};
		}
	}
}
