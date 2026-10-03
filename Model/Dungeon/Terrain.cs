using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public abstract class Terrain : IDrawable
    {
        public abstract char Symbol { get; }
        public abstract bool IsPassable { get; }
    }

    public class Wall : Terrain
    {
        public static readonly Wall Instance = new Wall();
        public override char Symbol => '█';
        public override bool IsPassable => false;
    }

    public class Floor : Terrain
    {
        public static readonly Floor Instance = new Floor();
        public override char Symbol => ' ';
        public override bool IsPassable => true;
    }

    public class Cell : IDrawable
    {
        public Terrain CellTerrain { get; private set; }
        public List<IItem> Items { get; } = new List<IItem>();

        public Cell(Terrain terrain) { CellTerrain = terrain; }

        public char Symbol => Items.Count > 0 ? Items.Last().Symbol : CellTerrain.Symbol;

        public bool CanEnter() => CellTerrain.IsPassable;
        public bool HasItems() => Items.Count > 0;
        public string GetTopItemName() => HasItems() ? Items.Last().Name : "";

        public IItem TakeTopItem()
        {
            if (!HasItems()) return null;
            IItem topItem = Items.Last();
            Items.RemoveAt(Items.Count - 1);
            return topItem;
        }

        public void AddItem(IItem item)
        {
            if (item != null) Items.Add(item);
        }

        public void SetTerrain(Terrain newTerrain)
        {
            CellTerrain = newTerrain;
            if (!CellTerrain.IsPassable) Items.Clear();
        }
    }

    public class Map
    {
        private readonly Cell[,] _grid;
        public int Width { get; }
        public int Height { get; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            _grid = new Cell[width, height];
        }

        public void SetCell(int x, int y, Cell cell) => _grid[x, y] = cell;
        public Cell GetCell(int x, int y) => IsInBounds(x, y) ? _grid[x, y] : null;
        public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
    }
}