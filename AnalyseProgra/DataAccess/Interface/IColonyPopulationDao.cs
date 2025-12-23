using AnalyseProgra.Models;

namespace AnalyseProgra.DataAccess.Interface
{
    public interface IColonyPopulationDao
    {
        ColonyPopulation? Get(int colonyId);
        ColonyPopulation Create(ColonyPopulation pop);
        void Update(ColonyPopulation pop);
        void Delete(int colonyId);
    }
}
