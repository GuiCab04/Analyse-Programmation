using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyBuildingDao
    {
        ColonyBuilding? GetById(int id);
        IEnumerable<ColonyBuilding> GetByColony(int colonyId);
        ColonyBuilding? GetByColonyAndType(int colonyId, int buildingTypeId);

        ColonyBuilding Create(ColonyBuilding building);
        void Update(ColonyBuilding building);
        void Delete(int id);
    }
}
