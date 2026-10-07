using AnalyseProgra.Models;
using AnalyseProgra.Models.Users;
using AnalyseProgra.Models.Enums;
using Microsoft.Data.Sqlite;
using AnalyseProgra.DataAccess.Dao;

using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.Models.Buildings;

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

            if (user.Role == UserRole.Player || user.Role == UserRole.Moderator)
            {
                IColonyDao colonyDao = new ColonyDao();
                IColonyBuildingStackDao stackDao = new ColonyBuildingStackDao();

                var colony = colonyDao.GetByOwner(user.Username, includeDetails: false).FirstOrDefault();
                if (colony == null)
                {
                    colony = colonyDao.Create(new Colony(user, $"{user.Username}'s Colony"));
                }

                colony.Population.CurrentPopulation = 0;
                colonyDao.Update(colony);

				var existingMine = stackDao.GetOne(colony.Id, BuildingType.SoluroMine, 1);
				if (existingMine == null)
				{
					var mine = new Mine(1, ResourceType.Soluro);
					mine.ColonyId = colony.Id;
					mine.Amount = 1;

					stackDao.Create(mine);
				}

				// Création d'une mairie niveau 1 de départ si elle n'existe pas
				var existingMairie = stackDao.GetOne(colony.Id, BuildingType.Mairie, 1);
				if (existingMairie == null)
				{
					var mairie = new Mairie(1);
					mairie.ColonyId = colony.Id;
					mairie.Amount = 1;

					stackDao.Create(mairie);
				}
			}
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
            var role = (UserRole)r.GetInt32(3);

            User user = role switch
            {
                UserRole.Admin => new User(r.GetString(1), r.GetString(2), this),
                UserRole.Moderator => new Player(r.GetString(1), r.GetString(2), this),
                UserRole.Player => new Player(r.GetString(1), r.GetString(2), this),
                _ => new User(r.GetString(1), r.GetString(2), this),
            };

            user.Id = r.GetInt32(0);
            user.Username = r.GetString(1);
            user.PasswordHash = r.GetString(2);
            user.Role = role;
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
            IColonyDao colonyDao = new ColonyDao();

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
