using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyResourceDao
    {
        ColonyResource? Get(int colonyId, string resourceTypeId);
        IEnumerable<ColonyResource> GetByColony(int colonyId);
        ColonyResource Create(ColonyResource cr);
        void Update(ColonyResource cr);
        void Delete(int colonyId, string resourceTypeId);
    }
}
