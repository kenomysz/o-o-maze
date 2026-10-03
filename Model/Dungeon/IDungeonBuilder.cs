using System;

namespace Project1
{
    public interface IDungeonBuilder
    {
        void BuildEmptyDungeon(int width, int height);
        void BuildFilledDungeon(int width, int height);
        void AddCorridors();
        void AddRooms();
        void AddCentralRoom(int width, int height);
        void AddItems(int count);
        void AddWeapons(int count);
        void AddEnemies(int count);

        void SetThemeIntro(string introMessage);
        void AddArtifact();
    }

    public class StandardDungeonBuilder : IDungeonBuilder
    {
        private Level _level;
        private readonly Random _rng = new Random();
        private readonly IDungeonTheme _theme;

        public StandardDungeonBuilder(IDungeonTheme theme)
        {
            _theme = theme;
        }

        private void EnsureInitialized()
        {
            if (_level == null) throw new InvalidOperationException("No level");
        }

        public void SetThemeIntro(string introMessage) { }

        public void BuildEmptyDungeon(int width, int height)
        {
            _level = new Level { Map = new Map(width, height) };
            FillWith(Floor.Instance);
        }

        public void BuildFilledDungeon(int width, int height)
        {
            _level = new Level { Map = new Map(width, height) };
            FillWith(Wall.Instance);
        }

        public void AddArtifact()
        {
            EnsureInitialized();
            SpawnOnFloor(_theme.CreateArtifact());
            _level.ContainsPickables = true;
        }

        public void AddItems(int count)
        {
            EnsureInitialized();
            if (count > 0) _level.ContainsPickables = true;
            for (int i = 0; i < count; i++) SpawnOnFloor(_theme.CreateRandomItem(_rng));
        }

        public void AddWeapons(int count)
        {
            EnsureInitialized();
            if (count > 0) _level.ContainsPickables = true;
            for (int i = 0; i < count; i++) SpawnOnFloor(_theme.CreateRandomWeapon(_rng));
        }

        public void AddEnemies(int count)
        {
            EnsureInitialized();
            int spawned = 0;
            int maxAttempts = count * 10;
            int attempts = 0;

            while (spawned < count && attempts < maxAttempts)
            {
                attempts++;
                int x = _rng.Next(_level.Map.Width);
                int y = _rng.Next(_level.Map.Height);
                Cell cell = _level.Map.GetCell(x, y);

                if (cell != null && cell.CanEnter() && _level.Entities.GetAt(x, y) == null)
                {
                    _level.Entities.Add(_theme.CreateEnemy(x, y, _rng, _level));
                    spawned++;
                }
            }
        }

        public void AddCorridors()
        {
            EnsureInitialized();

            for (int y = 1; y < _level.Map.Height - 1; y += 2)
            {
                for (int x = 1; x < _level.Map.Width - 1; x += 2)
                {
                    if (!_level.Map.GetCell(x, y).CanEnter())
                    {
                        Carve(x, y);
                    }
                }
            }
        }

        public void AddRooms()
        {
            EnsureInitialized();
            int numRooms = _rng.Next(3, 7);
            for (int i = 0; i < numRooms; i++)
            {
                int w = _rng.Next(4, 9), h = _rng.Next(4, 9);
                int startX = _rng.Next(1, _level.Map.Width - w - 1);
                int startY = _rng.Next(1, _level.Map.Height - h - 1);
                CarveArea(startX, startY, w, h);
            }
        }

        public void AddCentralRoom(int width, int height)
        {
            EnsureInitialized();
            CarveArea((_level.Map.Width / 2) - (width / 2), (_level.Map.Height / 2) - (height / 2), width, height);
        }

        public Level GetResult()
        {
            EnsureInitialized();
            return _level;
        }

        private void FillWith(Terrain terrain)
        {
            for (int x = 0; x < _level.Map.Width; x++)
                for (int y = 0; y < _level.Map.Height; y++)
                    _level.Map.SetCell(x, y, new Cell(terrain));
        }

        private void CarveArea(int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
                for (int y = startY; y < startY + height; y++)
                    _level.Map.GetCell(x, y)?.SetTerrain(Floor.Instance);
        }

        private bool IsInInnerBounds(int x, int y)
        {
            return x > 0 && x < _level.Map.Width - 1 && y > 0 && y < _level.Map.Height - 1;
        }

        private void Carve(int x, int y)
        {
            _level.Map.GetCell(x, y)?.SetTerrain(Floor.Instance);
            (int dx, int dy)[] directions = { (0, -2), (0, 2), (-2, 0), (2, 0) };
            Shuffle(directions);

            for (int i = 0; i < directions.Length; i++)
            {
                int nx = x + directions[i].dx;
                int ny = y + directions[i].dy;

                if (IsInInnerBounds(nx, ny) && !_level.Map.GetCell(nx, ny).CanEnter())
                {
                    _level.Map.GetCell(x + directions[i].dx / 2, y + directions[i].dy / 2)?.SetTerrain(Floor.Instance);
                    Carve(nx, ny);
                }
            }
        }

        private void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        private void SpawnOnFloor(IItem item)
        {
            Cell cell;
            do { cell = _level.Map.GetCell(_rng.Next(_level.Map.Width), _rng.Next(_level.Map.Height)); }
            while (cell == null || !cell.CanEnter());
            cell.Items.Add(item);
        }
    }
}