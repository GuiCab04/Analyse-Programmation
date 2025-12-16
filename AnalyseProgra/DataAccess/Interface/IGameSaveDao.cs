using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IGameSaveDao
    {
        GameSave? GetById(int id);
        IEnumerable<GameSave> GetByUser(int userId);
        IEnumerable<GameSave> GetByColony(int colonyId);
        GameSave? GetByUserAndName(int userId, string saveName);

        GameSave Create(GameSave save);
        void Update(GameSave save);
        void Delete(int id);
    }
}
