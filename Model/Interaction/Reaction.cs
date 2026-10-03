using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Project1
{
    public interface IReactionStrategy
    {
        string StatusName { get; }
        void ApplyEffect(Enemy enemy);
        void RevertEffect(Enemy enemy);
    }

    public class CowardlyReactionStrategy : IReactionStrategy
    {
        public string StatusName => "[PANICKING]";

        public void ApplyEffect(Enemy e)
        {
            e.AttackPower = Math.Max(1, e.AttackPower - 2);
            e.Armor = Math.Max(0, e.Armor - 1);
            Journal.Instance.Log(new GameLog($"{e.Name} panics! Stats decreased."));
        }

        public void RevertEffect(Enemy e)
        {
            e.AttackPower += 2;
            e.Armor += 1;
            Journal.Instance.Log(new GameLog($"{e.Name} calms down. Stats returned to normal."));
        }
    }

    public class AggressiveReactionStrategy : IReactionStrategy
    {
        public string StatusName => "[ENRAGED]";

        public void ApplyEffect(Enemy e)
        {
            e.AttackPower += 2;
            e.Armor += 2;
            Journal.Instance.Log(new GameLog($"{e.Name} enters a battle rage! Stats increased."));
        }

        public void RevertEffect(Enemy e)
        {
            e.AttackPower -= 2;
            e.Armor -= 2;
            Journal.Instance.Log(new GameLog($"{e.Name}'s battle rage fades. Stats returned to normal."));
        }
    }

    public interface IEnemyMovementStrategy
    {
        void ExecuteMove(Enemy enemy, Level level, Random rng);
    }

    public class RandomMovementStrategy : IEnemyMovementStrategy
    {
        public void ExecuteMove(Enemy enemy, Level level, Random rng)
        {
            (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };
            var dir = directions[rng.Next(directions.Length)];
            int nx = enemy.Pos.X + dir.dx;
            int ny = enemy.Pos.Y + dir.dy;

            if (level.Map.IsInBounds(nx, ny) && level.Map.GetCell(nx, ny).CanEnter() && level.Entities.GetAt(nx, ny) == null)
            {
                level.Entities.UpdatePosition(enemy, nx, ny);
                enemy.Move(nx, ny);
            }
        }
    }


}