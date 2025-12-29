using System.Collections.Generic;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyResourceDao
    {
        IEnumerable<ColonyResource> GetByColonyId(int colonyId);
        ColonyResource? GetOne(int colonyId, ResourceTypeEnums resourceType);
        ColonyResource Create(ColonyResource resource);
        void Update(ColonyResource resource);
        void Delete(int colonyId, ResourceTypeEnums resourceType);
        void DeleteByColony(int colonyId);
    }
}
