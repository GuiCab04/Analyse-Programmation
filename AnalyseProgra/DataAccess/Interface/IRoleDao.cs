using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IRoleDao
    {
        Role? GetById(int id);
        Role? GetByName(string name);
        IEnumerable<Role> GetAll();

        Role Create(Role role);
        void Update(Role role);
        void Delete(int id);
    }
}
