using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IModerationActionDao
    {
        ModerationAction? GetById(int id);
        IEnumerable<ModerationAction> GetByTargetUser(int targetUserId);
        IEnumerable<ModerationAction> GetByPerformer(int performedByUserId);
        IEnumerable<ModerationAction> GetAll();

        ModerationAction Create(ModerationAction action);
        void Update(ModerationAction action);
        void Delete(int id);
    }
}