using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IBuildingTypeDao
    {
        BuildingType? GetById(int id);
        BuildingType? GetByName(string name);
        IEnumerable<BuildingType> GetAll();
        BuildingType Create(BuildingType type);
        void Update(BuildingType type);
        void Delete(int id);
    }
}
