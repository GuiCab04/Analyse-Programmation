using System.Collections.Generic;
using AnalyseProgra.Models;
using AnalyseProgra.Models.Enums;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyBuildingStackDao
    {
        IEnumerable<ColonyBuildingStack> GetByColonyId(int colonyId);
        ColonyBuildingStack? GetOne(int colonyId, BuildingType buildingType, int level);
        ColonyBuildingStack Create(ColonyBuildingStack stack);
        void Update(ColonyBuildingStack stack);
        void Delete(int colonyId, BuildingType buildingType, int level);
        void DeleteByColony(int colonyId);
    }
}
