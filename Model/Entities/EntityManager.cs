using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public class EntityManager
    {
        private readonly Dictionary<(int X, int Y), List<Character>> _entities = new Dictionary<(int, int), List<Character>>();

        public void Add(Character entity)
        {
            if (entity == null) return;

            var key = (entity.Pos.X, entity.Pos.Y);
            if (!_entities.ContainsKey(key))
            {
                _entities[key] = new List<Character>();
            }
            _entities[key].Add(entity);
        }

        public void Remove(Character entity)
        {
            if (entity == null) return;

            var key = (entity.Pos.X, entity.Pos.Y);
            if (_entities.TryGetValue(key, out var list))
            {
                list.Remove(entity);
                if (list.Count == 0)
                {
                    _entities.Remove(key);
                }
            }
        }

        public void UpdatePosition(Character entity, int newX, int newY)
        {
            if (entity == null) return;

            var oldKey = (entity.Pos.X, entity.Pos.Y);
            if (_entities.TryGetValue(oldKey, out var oldList))
            {
                oldList.Remove(entity);
                if (oldList.Count == 0)
                {
                    _entities.Remove(oldKey);
                }
            }

            var newKey = (newX, newY);
            if (!_entities.ContainsKey(newKey))
            {
                _entities[newKey] = new List<Character>();
            }
            _entities[newKey].Add(entity);
        }

        public Character GetAt(int x, int y)
        {
            if (_entities.TryGetValue((x, y), out var list) && list.Count > 0)
            {
                return list.Last();
            }
            return null;
        }

        public IEnumerable<Character> GetAll()
        {
            List<Character> all = new List<Character>();
            foreach (var list in _entities.Values)
            {
                all.AddRange(list);
            }
            return all;
        }
    }
}