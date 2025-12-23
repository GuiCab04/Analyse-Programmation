using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyBuildingDao
    {
        IEnumerable<ColonyBuilding> GetByColony(int colonyId);
        ColonyBuilding? GetByColonyAndType(int colonyId, string buildingTypeId);

        ColonyBuilding Create(ColonyBuilding building);
        void Update(ColonyBuilding building);

        void Delete(int colonyId, string buildingTypeId);
    }
}
