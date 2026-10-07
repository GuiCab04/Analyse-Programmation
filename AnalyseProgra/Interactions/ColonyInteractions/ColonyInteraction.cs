using AnalyseProgra.Models;

namespace AnalyseProgra.Interactions.ColonyInteractions
{
    public abstract class ColonyInteraction : Interaction
    {
        protected Colony _colony;

        protected ColonyInteraction(Colony colony, string name, string description = "")
            : base(name, description)
        {
            _colony = colony;
        }
    }
}
