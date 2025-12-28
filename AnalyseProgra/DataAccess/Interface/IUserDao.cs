using System.Collections.Generic;
using AnalyseProgra.Models;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IUserDao
    {
        User? GetById(int id, bool includeDetails = false);
        User? GetByUsername(string username, bool includeDetails = false);
        IEnumerable<User> GetAll();
        User Create(User user);
        void Update(User user);
        void Delete(int id);
    }
}
