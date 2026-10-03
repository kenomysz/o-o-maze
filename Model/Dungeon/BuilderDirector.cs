using System.Collections.Generic;

namespace Project1
{
    public class DungeonPackage
    {
        public List<string> Story { get; set; }
        public List<string> Controls { get; set; }
        public Level Level { get; set; }
        public List<string> Manual { get; set; }
    }

    public class DungeonDirector
    {
        private readonly IDungeonBuilder _builder;
        private readonly IDungeonTheme _theme;

        public DungeonDirector(IDungeonBuilder builder, IDungeonTheme theme)
        {
            _builder = builder;
            _theme = theme;
        }

        public void BuildTerrainStrategy(int width, int height)
        {
            _builder.SetThemeIntro(_theme.IntroMessage);
            _theme.Template.GenerateLayout(_builder, width, height);

            _builder.AddItems(6);
            _builder.AddWeapons(7);
            _builder.AddArtifact();
            _builder.AddEnemies(8);
        }

        public void BuildNoItems(int width, int height)
        {
            _builder.SetThemeIntro(_theme.IntroMessage);
            _theme.Template.GenerateLayout(_builder, width, height);
        }
    }

    public class DungeonFacade
    {
        public DungeonPackage CreateFullDungeon(int width, int height, IDungeonTheme theme)
        {
            var mapBuilder = new StandardDungeonBuilder(theme);
            var mapDirector = new DungeonDirector(mapBuilder, theme);
            mapDirector.BuildTerrainStrategy(width, height);

            var manualBuilder = new InstructionBuilder(theme);
            var manualDirector = new DungeonDirector(manualBuilder, theme);
            manualDirector.BuildTerrainStrategy(width, height);

            return new DungeonPackage
            {
                Level = mapBuilder.GetResult(),
                Manual = manualBuilder.GetFullManual(),
                Controls = manualBuilder.GetControls(),
                Story = manualBuilder.GetStory()
            };
        }

        public DungeonPackage CreateEmpty(int width, int height, IDungeonTheme theme)
        {
            var mapBuilder = new StandardDungeonBuilder(theme);
            var mapDirector = new DungeonDirector(mapBuilder, theme);
            mapDirector.BuildNoItems(width, height);

            var manualBuilder = new InstructionBuilder(theme);
            var manualDirector = new DungeonDirector(manualBuilder, theme);
            manualDirector.BuildNoItems(width, height);

            return new DungeonPackage
            {
                Level = mapBuilder.GetResult(),
                Manual = manualBuilder.GetFullManual(),
                Controls = manualBuilder.GetControls(),
                Story = manualBuilder.GetStory()
            };
        }
    }
}