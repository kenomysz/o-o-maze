using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{

    public enum CollisionResult
    {
        Allowed,
        Blocked,
        StartCombat
    }
    public abstract class Character : Entity, IDamageable
    {
        public Attributes Stats { get; protected set; } = new Attributes(10, 10, 10, 10, 10);
        public HealthBar HP { get; protected set; } = new HealthBar(100, 100);

        public virtual bool IsControlledByPlayer => false;



        public virtual CollisionResult HandleCollision(Player movingPlayer)
        {
            return CollisionResult.Allowed;
        }
        public Character(int startX, int startY, string name, char symbol) : base(startX, startY, name, symbol) { }

        public bool IsDead { get; protected set; } = false;
        public void TakeDamage(int amount)
        {
            HP.ChangeHealth(-amount);
            if (HP.Health <= 0) this.Die();
        }

        public virtual IEnumerable<IItem> GetEquippedItems()
        {
            yield break;
        }

        public virtual void Die()
        {
            IsDead = true;
        }
    }
}
