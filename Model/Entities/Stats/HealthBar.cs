using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class HealthBar
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }

        public HealthBar(int health = 0, int maxHealth = 0)
        {
            Health = health;
            MaxHealth = maxHealth;
        }

        public void ChangeHealth(int amount)
        {
            Health = Math.Max(Health + amount, 0);
            Health = Math.Min(Health, MaxHealth);
        }
    }

}
