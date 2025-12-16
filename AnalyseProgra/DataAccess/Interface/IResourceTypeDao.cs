using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyseProgra.Models;


namespace AnalyseProgra.DataAccess.Interface
{
    public interface IResourceTypeDao
    {
        ResourceType? GetById(int id);
        ResourceType? GetByName(string name);
        IEnumerable<ResourceType> GetAll();
        ResourceType Create(ResourceType type);
        void Update(ResourceType type);
        void Delete(int id);
    }
}
