using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IBuildingTypeCostDao
    {
        BuildingTypeCost? Get(string buildingTypeId, string resourceTypeId);
        IEnumerable<BuildingTypeCost> GetByBuildingType(string buildingTypeId);

        BuildingTypeCost Create(BuildingTypeCost cost);
        void Update(BuildingTypeCost cost);
        void Delete(string buildingTypeId, string resourceTypeId);
    }
}
