using AnalyseProgra.Models;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Models.Enums;
using Microsoft.Data.Sqlite;

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

        private User Map(SqliteDataReader r)
        {
            var Role = (UserRole)r.GetInt32(3);
            User user;
            switch (Role)
            {
                case UserRole.Admin:
                    throw new NotImplementedException("Admin user mapping not implemented yet.");
                    break;
                case UserRole.Player:
                    user = new Player(r.GetString(1), r.GetString(2), this);
                    break;
                default:
                    user = new User(r.GetString(1), r.GetString(2), this);
                    break;
            }

            user.Id = r.GetInt32(0);
            user.Username = r.GetString(1);
            user.PasswordHash = r.GetString(2);
            user.Role = Role;
            user.IsActive = r.GetInt32(4) != 0;

            return user;
        }

        private static void LoadRelations(User user)
        {
            var colonyDao = new ColonyDao();
            var colony = colonyDao.GetByOwner(user.Username, includeDetails: true).ToList().FirstOrDefault();
            if (colony != null && user is Player)
                ((Player)user).Colony = colony;
        }

        public void SaveWithRelations(User user)
        {
            var colonyDao = new ColonyDao();

            if (user.Id == 0)
            {
                Create(user);
            }
            else
            {
                Update(user);
            };

            if (user is Player)
            {
                colonyDao.SaveWithRelations(((Player)user).Colony);
            }
        }
    }
}
