using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class CombatResult
    {
        public int Damage { get; set; }
        public int Defense { get; set; }
    }

    public interface ICombatAttack
    {
        CombatResult VisitHeavy(HeavyWeapon w, Attributes stats, int damageModifier);
        CombatResult VisitLight(LightWeapon w, Attributes stats, int damageModifier);
        CombatResult VisitMagic(MagicWeapon w, Attributes stats, int damageModifier);
        CombatResult VisitOther(IItem w, Attributes stats, int damageModifier);
    }

    public class NormalAttack : ICombatAttack
    {
        public CombatResult VisitHeavy(HeavyWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = w.GetBaseDamage() + dmgMod + stats.Strength + stats.Aggression,
            Defense = stats.Strength + stats.Luck
        };

        public CombatResult VisitLight(LightWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = w.GetBaseDamage() + dmgMod + stats.Dexterity + stats.Luck,
            Defense = stats.Dexterity + stats.Luck
        };

        public CombatResult VisitMagic(MagicWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = 1,
            Defense = stats.Dexterity + stats.Luck
        };

        public CombatResult VisitOther(IItem w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = 0,
            Defense = stats.Dexterity
        };
    }

    public class StealthAttack : ICombatAttack
    {
        public CombatResult VisitHeavy(HeavyWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = (w.GetBaseDamage() + dmgMod + stats.Strength + stats.Aggression) / 2,
            Defense = stats.Strength
        };

        public CombatResult VisitLight(LightWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = (w.GetBaseDamage() + dmgMod + stats.Dexterity + stats.Luck) * 2,
            Defense = stats.Dexterity
        };

        public CombatResult VisitMagic(MagicWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = 1,
            Defense = 0
        };

        public CombatResult VisitOther(IItem w, Attributes stats, int dmgMod) => new CombatResult { Damage = 0, Defense = 0 };
    }

    public class MagicAttack : ICombatAttack
    {
        public CombatResult VisitHeavy(HeavyWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = 1,
            Defense = stats.Luck
        };

        public CombatResult VisitLight(LightWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = 1,
            Defense = stats.Luck
        };

        public CombatResult VisitMagic(MagicWeapon w, Attributes stats, int dmgMod) => new CombatResult
        {
            Damage = w.GetBaseDamage() + dmgMod + stats.Wisdom,
            Defense = stats.Wisdom * 2
        };

        public CombatResult VisitOther(IItem w, Attributes stats, int dmgMod) => new CombatResult { Damage = 0, Defense = stats.Luck };
    }
}