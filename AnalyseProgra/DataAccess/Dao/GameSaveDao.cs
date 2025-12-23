using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class GameSaveDao : IGameSaveDao
    {
        public GameSave? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, colony_id, save_name, data_json
                FROM game_saves
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<GameSave> GetByUser(int userId)
        {
            var list = new List<GameSave>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, colony_id, save_name, data_json
                FROM game_saves
                WHERE user_id = $uid;
            ";
            cmd.Parameters.AddWithValue("$uid", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public IEnumerable<GameSave> GetByColony(int colonyId)
        {
            var list = new List<GameSave>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, colony_id, save_name, data_json
                FROM game_saves
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

        public GameSave? GetByUserAndName(int userId, string saveName)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, user_id, colony_id, save_name, data_json
                FROM game_saves
                WHERE user_id = $uid AND save_name = $name;
            ";
            cmd.Parameters.AddWithValue("$uid", userId);
            cmd.Parameters.AddWithValue("$name", saveName);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public GameSave Create(GameSave save)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO game_saves (user_id, colony_id, save_name, data_json)
                VALUES ($uid, $cid, $name, $json);

                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$uid", save.UserId);
            cmd.Parameters.AddWithValue("$cid", save.ColonyId);
            cmd.Parameters.AddWithValue("$name", save.SaveName);
            cmd.Parameters.AddWithValue("$json", save.DataJson);

            var newId = (long)cmd.ExecuteScalar();
            save.Id = (int)newId;

            return save;
        }

        public void Update(GameSave save)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE game_saves
                SET user_id   = $uid,
                    colony_id = $cid,
                    save_name = $name,
                    data_json = $json
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$uid", save.UserId);
            cmd.Parameters.AddWithValue("$cid", save.ColonyId);
            cmd.Parameters.AddWithValue("$name", save.SaveName);
            cmd.Parameters.AddWithValue("$json", save.DataJson);
            cmd.Parameters.AddWithValue("$id", save.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM game_saves WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static GameSave Map(SqliteDataReader r)
        {
            return new GameSave
            {
                Id = r.GetInt32(0),
                UserId = r.GetInt32(1),
                ColonyId = r.GetInt32(2),
                SaveName = r.GetString(3),
                DataJson = r.GetString(4)
            };
        }
    }
}
