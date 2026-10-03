using System.Collections.Generic;

namespace Project1
{
    public class InstructionBuilder : IDungeonBuilder
    {
        private readonly IDungeonTheme _theme;
        private readonly List<string> _storyParts = new List<string>();
        private readonly List<string> _controlParts = new List<string>();

        public InstructionBuilder(IDungeonTheme theme)
        {
            _theme = theme;
        }

        private void AddControl(string text)
        {
            if (!_controlParts.Contains(text)) _controlParts.Add(text);
        }

        public void SetThemeIntro(string introMessage) => _storyParts.Insert(0, introMessage);

        public void AddArtifact() => _storyParts.Add($"Here lies {_theme.ArtifactLore}.");

        public void BuildEmptyDungeon(int width, int height)
        {
            _storyParts.Add($"This is a vast, open dungeon of {width}x{height} size.");
            AddStandardControls();
        }

        public void BuildFilledDungeon(int width, int height)
        {
            _storyParts.Add($"You enter a dense dungeon of {width}x{height} size, {_theme.TerrainLore}.");
            AddStandardControls();
        }

        public void AddStandardControls()
        {
            AddControl($"[{ControlsConfig.MoveUp},{ControlsConfig.MoveDown},{ControlsConfig.MoveLeft},{ControlsConfig.MoveRight}] - Movement");
        }

        public void AddCorridors() => _storyParts.Add("Narrow corridors connect distant parts of the complex.");
        public void AddRooms() => _storyParts.Add("Mysterious rooms are scattered throughout the area.");
        public void AddCentralRoom(int w, int h) => _storyParts.Add($"A grand central hall ({w}x{h}) lies in the heart of the dungeon.");

        public void AddItems(int count)
        {
            if (count > 0)
            {
                _storyParts.Add($"You can find {count} {_theme.ItemLore} scattered on the floor.");
                AddControl($"[{ControlsConfig.PickUp}] - Pick up item");
                AddControl($"[{ControlsConfig.Inventory}] - Inventory");
            }
        }

        public void AddWeapons(int count)
        {
            if (count > 0)
            {
                _storyParts.Add($"The armory reports {count} {_theme.WeaponLore} hidden in the shadows.");
                AddControl($"[{ControlsConfig.PickUp}] - Pick up item");
                AddControl($"[{ControlsConfig.Inventory}] - Inventory");
            }
        }

        public void AddEnemies(int count)
        {
            if (count > 0)
            {
                _storyParts.Add($"Beware! {count} {_theme.EnemyLore} are lurking in the darkness.");
                AddControl("Walk into an enemy to initiate combat.");
            }
        }

        public List<string> GetStory() => new List<string>(_storyParts);
        public List<string> GetControls() => new List<string>(_controlParts);

        public List<string> GetFullManual()
        {
            var finalResult = new List<string>();
            finalResult.AddRange(_storyParts);
            finalResult.Add("");
            finalResult.AddRange(_controlParts);
            return finalResult;
        }
    }
}