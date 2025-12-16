using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnalyseProgra.Models;
using System.Collections.Generic;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyResourceDao
    {
        ColonyResource? Get(int colonyId, int resourceTypeId);
        IEnumerable<ColonyResource> GetByColony(int colonyId);
        ColonyResource Create(ColonyResource cr);
        void Update(ColonyResource cr);
        void Delete(int colonyId, int resourceTypeId);
    }
}
