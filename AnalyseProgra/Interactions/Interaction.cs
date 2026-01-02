using AnalyseProgra.UserInterfaces;

namespace AnalyseProgra.Interactions
{
    public abstract class Interaction
    {
        public string Name { get; }
        public string Description { get; }

        protected Interaction(string name, string description = "")
        {
            Name = name;
            Description = description;
        }

        public abstract void Execute(IUserInterface input);

        public override string ToString() => $"{Name}: {Description}";
    }
}
