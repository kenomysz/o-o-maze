using System;
using System.Collections.Generic;

namespace Project1
{
    public interface IGenerationTemplate
    {
        void GenerateLayout(IDungeonBuilder builder, int width, int height);
    }

    public class LibraryGenerationTemplate : IGenerationTemplate
    {
        public void GenerateLayout(IDungeonBuilder builder, int width, int height)
        {
            builder.BuildFilledDungeon(width, height);
            builder.AddCorridors();
        }
    }

    public class FactoryGenerationTemplate : IGenerationTemplate
    {
        public void GenerateLayout(IDungeonBuilder builder, int width, int height)
        {
            builder.BuildFilledDungeon(width, height);
            builder.AddRooms();
        }
    }

    public class VaultGenerationTemplate : IGenerationTemplate
    {
        public void GenerateLayout(IDungeonBuilder builder, int width, int height)
        {
            builder.BuildFilledDungeon(width, height);
            builder.AddCorridors();
            builder.AddCentralRoom(15, 15);
        }
    }

    public interface IDungeonTheme
    {
        string IntroMessage { get; }

        string TerrainLore { get; }
        string ItemLore { get; }
        string WeaponLore { get; }
        string EnemyLore { get; }
        string ArtifactLore { get; }

        IGenerationTemplate Template { get; }

        IItem CreateRandomItem(Random rng);
        IItem CreateRandomWeapon(Random rng);
        IItem CreateArtifact();
        Enemy CreateEnemy(int x, int y, Random rng, Level level);
    }

    public class LibraryTheme : IDungeonTheme
    {
        public string IntroMessage => "\"The smell of old books fills the air\"";
        public string TerrainLore => "forming a massive labyrinth of towering bookshelves";
        public string ItemLore => "dusty scrolls and strange bottles";
        public string WeaponLore => "magical staves and ritual daggers";
        public string EnemyLore => "dark mages and flying cursed tomes";
        public string ArtifactLore => "an ancient, legendary grimoire";

        public IGenerationTemplate Template { get; } = new LibraryGenerationTemplate();

        private readonly ItemFactory _itemFactory;
        private readonly IEnemyFactory _enemyFactory;

        public LibraryTheme()
        {
            var allowedWeapons = new List<Func<IItem>> { () => new MagicStaff(), () => new Dagger() };
            var allowedMisc = new List<Func<IItem>> { () => new Scroll(), () => new Bottle() };
            _itemFactory = new ItemFactory(allowedWeapons, allowedMisc);

            _enemyFactory = new LibraryEnemyFactory();
        }

        public IItem CreateRandomItem(Random rng) => _itemFactory.CreateRandomMiscItem();
        public IItem CreateRandomWeapon(Random rng) => _itemFactory.CreateRandomWeapon();
        public IItem CreateArtifact() => new ArtifactModifier(new MagicStaff());
        public Enemy CreateEnemy(int x, int y, Random rng, Level level) => _enemyFactory.CreateRandomEnemy(x, y, rng, level);
    }

    public class FactoryTheme : IDungeonTheme
    {
        public string IntroMessage => "\"The grinding of metal echoes off the walls\"";

        public string TerrainLore => "consisting of rusty metal bulkheads and steaming pipes";
        public string ItemLore => "metal scraps and mechanical parts";
        public string WeaponLore => "heavy swords and sharp metal blades";
        public string EnemyLore => "rogue robots and scrap workers";
        public string ArtifactLore => "a masterfully crafted energy dagger";

        public IGenerationTemplate Template { get; } = new FactoryGenerationTemplate();

        private readonly ItemFactory _itemFactory;
        private readonly IEnemyFactory _enemyFactory;

        public FactoryTheme()
        {
            var allowedWeapons = new List<Func<IItem>> { () => new LongSword(), () => new ShortSword() };
            var allowedMisc = new List<Func<IItem>> { () => new Stick() };
            _itemFactory = new ItemFactory(allowedWeapons, allowedMisc);

            _enemyFactory = new FactoryEnemyFactory();
        }

        public IItem CreateRandomItem(Random rng) => _itemFactory.CreateRandomMiscItem();
        public IItem CreateRandomWeapon(Random rng) => _itemFactory.CreateRandomWeapon();
        public IItem CreateArtifact() => new ArtifactModifier(new Dagger());
        public Enemy CreateEnemy(int x, int y, Random rng, Level level) => _enemyFactory.CreateRandomEnemy(x, y, rng, level);
    }

    public class VaultTheme : IDungeonTheme
    {
        public string IntroMessage => "\"You smell coins.\"";

        public string TerrainLore => "lined with thick, impenetrable steel walls";
        public string ItemLore => "piles of gold and scattered coins";
        public string WeaponLore => "ornate longswords left by previous guards";
        public string EnemyLore => "greedy bandits and aggressive iron safes";
        public string ArtifactLore => "a priceless, jeweled longsword";

        public IGenerationTemplate Template { get; } = new VaultGenerationTemplate();

        private readonly ItemFactory _itemFactory;
        private readonly IEnemyFactory _enemyFactory;

        public VaultTheme()
        {
            var allowedWeapons = new List<Func<IItem>> { () => new LongSword(), () => new Dagger() };
            var allowedMisc = new List<Func<IItem>> { () => new Gold(new Random().Next(10, 50)), () => new Coins(new Random().Next(1, 20)) };
            _itemFactory = new ItemFactory(allowedWeapons, allowedMisc);

            _enemyFactory = new VaultEnemyFactory();
        }

        public IItem CreateRandomItem(Random rng) => _itemFactory.CreateRandomMiscItem();
        public IItem CreateRandomWeapon(Random rng) => _itemFactory.CreateRandomWeapon();
        public IItem CreateArtifact() => new ArtifactModifier(new LongSword());
        public Enemy CreateEnemy(int x, int y, Random rng, Level level) => _enemyFactory.CreateRandomEnemy(x, y, rng, level);
    }
}
