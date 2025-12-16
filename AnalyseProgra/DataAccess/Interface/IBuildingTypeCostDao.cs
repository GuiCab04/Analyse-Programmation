using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IBuildingTypeCostDao
    {
        BuildingTypeCost? Get(int buildingTypeId, int resourceTypeId);
        IEnumerable<BuildingTypeCost> GetByBuildingType(int buildingTypeId);

        BuildingTypeCost Create(BuildingTypeCost cost);
        void Update(BuildingTypeCost cost);
        void Delete(int buildingTypeId, int resourceTypeId);
    }
}
