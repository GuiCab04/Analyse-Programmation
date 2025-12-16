using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ModerationActionDao : IModerationActionDao
    {
        public ModerationAction? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, performed_by_user_id, target_user_id, action_type, details
                FROM moderation_actions
                WHERE id = $id;
            ";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<ModerationAction> GetByTargetUser(int targetUserId)
        {
            var list = new List<ModerationAction>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, performed_by_user_id, target_user_id, action_type, details
                FROM moderation_actions
                WHERE target_user_id = $tid;
            ";
            cmd.Parameters.AddWithValue("$tid", targetUserId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public IEnumerable<ModerationAction> GetByPerformer(int performedByUserId)
        {
            var list = new List<ModerationAction>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, performed_by_user_id, target_user_id, action_type, details
                FROM moderation_actions
                WHERE performed_by_user_id = $pid;
            ";
            cmd.Parameters.AddWithValue("$pid", performedByUserId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public IEnumerable<ModerationAction> GetAll()
        {
            var list = new List<ModerationAction>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT id, performed_by_user_id, target_user_id, action_type, details
                FROM moderation_actions;
            ";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public ModerationAction Create(ModerationAction action)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO moderation_actions (performed_by_user_id, target_user_id, action_type, details)
                VALUES ($pid, $tid, $type, $details);

                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$pid", action.PerformedByUserId);
            cmd.Parameters.AddWithValue("$tid", action.TargetUserId);
            cmd.Parameters.AddWithValue("$type", action.ActionType);
            cmd.Parameters.AddWithValue("$details", (object?)action.Details ?? System.DBNull.Value);

            var newId = (long)cmd.ExecuteScalar();
            action.Id = (int)newId;

            return action;
        }

        public void Update(ModerationAction action)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE moderation_actions
                SET performed_by_user_id = $pid,
                    target_user_id       = $tid,
                    action_type          = $type,
                    details              = $details
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$pid", action.PerformedByUserId);
            cmd.Parameters.AddWithValue("$tid", action.TargetUserId);
            cmd.Parameters.AddWithValue("$type", action.ActionType);
            cmd.Parameters.AddWithValue("$details", (object?)action.Details ?? System.DBNull.Value);
            cmd.Parameters.AddWithValue("$id", action.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM moderation_actions WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static ModerationAction Map(SqliteDataReader r)
        {
            return new ModerationAction
            {
                Id = r.GetInt32(0),
                PerformedByUserId = r.GetInt32(1),
                TargetUserId = r.GetInt32(2),
                ActionType = r.GetString(3),
                Details = r.IsDBNull(4) ? null : r.GetString(4)
            };
        }
    }
}
