using System.Collections.Generic;
using AnalyseProgra.Models;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyDao
    {
        Colony? GetById(int id, bool includeDetails = false);
        IEnumerable<Colony> GetByOwner(string ownerUsername, bool includeDetails = false);
        IEnumerable<Colony> GetAll(bool includeDetails = false);
        Colony Create(Colony colony);
        void Update(Colony colony);
        void Delete(int id);
    }
}
