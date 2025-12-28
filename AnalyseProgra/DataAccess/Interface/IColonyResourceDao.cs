using System.Collections.Generic;
using AnalyseProgra.Models;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyResourceDao
    {
        IEnumerable<ColonyResource> GetByColonyId(int colonyId);
        ColonyResource? GetOne(int colonyId, string resourceName);
        ColonyResource Create(ColonyResource resource);
        void Update(ColonyResource resource);
        void Delete(int colonyId, string resourceName);
        void DeleteByColony(int colonyId);
    }
}
