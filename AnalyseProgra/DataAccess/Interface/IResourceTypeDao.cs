using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IResourceTypeDao
    {
        ResourceType? GetByName(string name);
        IEnumerable<ResourceType> GetAll();
        ResourceType Create(ResourceType type);

        // optionnel (voir avertissement)
        void Update(string name, ResourceType updated);

        void Delete(string name);
    }
}
