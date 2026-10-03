using System;

namespace Project1
{
    public interface IEnemyFactory
    {
        Enemy CreateRandomEnemy(int x, int y, Random rng, Level level);
    }

    public class LibraryEnemyFactory : IEnemyFactory
    {
        private readonly Faction _mageFaction = new Faction("Dark Mages", new CowardlyReactionStrategy());
        private readonly Faction _tomeFaction = new Faction("Cursed Tomes", new AggressiveReactionStrategy());

        public Enemy CreateRandomEnemy(int x, int y, Random rng, Level level)
        {
            if (rng.NextDouble() < 0.6)
            {
                return new Enemy(x, y, "Dark Mage", 'M', 30, 8, 2, new Attributes(5, 5, 5, 5, 5), _mageFaction, level, new RandomMovementStrategy());
            }

            return new Enemy(x, y, "Cursed Tome", 'T', 20, 6, 1, new Attributes(3, 3, 3, 3, 8), _tomeFaction, level, new RandomMovementStrategy());
        }
    }

    public class FactoryEnemyFactory : IEnemyFactory
    {
        private readonly Faction _workerFaction = new Faction("Scrap Workers", new CowardlyReactionStrategy());
        private readonly Faction _defenseFaction = new Faction("Defense Units", new AggressiveReactionStrategy());

        public Enemy CreateRandomEnemy(int x, int y, Random rng, Level level)
        {
            if (rng.NextDouble() < 0.6)
            {
                return new Enemy(x, y, "Scrap Bot", 'b', 40, 10, 3, new Attributes(4, 4, 4, 4, 4), _workerFaction, level, new RandomMovementStrategy());
            }

            return new Enemy(x, y, "Rogue Robot", 'R', 50, 12, 5, new Attributes(5, 5, 5, 5, 5), _defenseFaction, level, new RandomMovementStrategy());
        }
    }

    public class VaultEnemyFactory : IEnemyFactory
    {
        private readonly Faction _thiefFaction = new Faction("Thieves", new CowardlyReactionStrategy());
        private readonly Faction _guardianFaction = new Faction("Iron Guardians", new AggressiveReactionStrategy());

        public Enemy CreateRandomEnemy(int x, int y, Random rng, Level level)
        {
            if (rng.NextDouble() < 0.7)
            {
                return new Enemy(x, y, "Bandit", 'B', 45, 9, 2, new Attributes(6, 6, 6, 6, 2), _thiefFaction, level, new RandomMovementStrategy());
            }

            return new Enemy(x, y, "Aggressive Safe", 'S', 100, 5, 10, new Attributes(5, 5, 5, 5, 5), _guardianFaction, level, new RandomMovementStrategy());
        }
    }
}