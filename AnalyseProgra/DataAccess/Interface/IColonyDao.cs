using AnalyseProgra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyDao
    {
        Colony? GetById(int id);
        Colony? GetByUserId(int userId);
        IEnumerable<Colony> GetAll();
        Colony Create(Colony colony);
        void Update(Colony colony);
        void Delete(int id);
    }
}
