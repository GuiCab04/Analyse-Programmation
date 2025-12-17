using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IBuildingTypeDao
    {
        BuildingType? GetByName(string name);
        IEnumerable<BuildingType> GetAll();
        BuildingType Create(BuildingType type);

        // Si tu veux permettre renommage:
        void Update(string name, BuildingType updated);

        void Delete(string name);
    }
}
