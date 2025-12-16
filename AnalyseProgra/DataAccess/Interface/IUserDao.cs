using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IUserDao
    {
        User? GetById(int id);
        User? GetByUsername(string username);
        IEnumerable<User> GetAll();

        User Create(User user);
        void Update(User user);
        void Delete(int id);
    }
}
