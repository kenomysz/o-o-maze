using System.Collections.Generic;

namespace Project1
{
    public class Faction : ISpeciesSubject
    {
        public string Name { get; }
        public IReactionStrategy ReactionStrategy { get; } 

        private readonly List<ISpeciesObserver> _members = new List<ISpeciesObserver>();

        public Faction(string name, IReactionStrategy reactionStrategy)
        {
            Name = name;
            ReactionStrategy = reactionStrategy;
        }

        public void Attach(ISpeciesObserver observer) => _members.Add(observer);
        public void Detach(ISpeciesObserver observer) => _members.Remove(observer);

        public void NotifyAllyDeath(ISpeciesObserver deceased)
        {
            var currentMembers = new List<ISpeciesObserver>(_members);
            foreach (var member in currentMembers)
            {
                if (member != deceased)
                {
                    member.OnAllyDied();
                }
            }
        }
    }
}