using System;

namespace Project1
{
    public interface IItem : IDrawable
    {
        string Name { get; }
        int EquipSlotSize { get; }
        bool IsStorable { get; }

        int GetBaseDamage();
        int GetNoiseLevel();
        void OnPickUp(Player player);

        string GetInfo();
        string GetInstruction();

        bool EquipTo(EquipSlots hands);

        Attributes ModifyAttributes(Attributes current);

        CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0);
    }

    public abstract class Item : IItem
    {
        public virtual string Name { get; protected set; }
        public virtual char Symbol { get; protected set; }
        public virtual int EquipSlotSize => 0;
        public virtual bool IsStorable => true;

        public virtual void OnPickUp(Player player) { }

        public virtual string GetInfo() => Name;
        public virtual string GetInstruction() => $"Pick up: {Name}";

        public virtual int GetBaseDamage() => 0;
        public virtual int GetNoiseLevel() => 0;

        public virtual bool EquipTo(EquipSlots hands) => false;

        public virtual Attributes ModifyAttributes(Attributes current) => current;

        public virtual CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => attack.VisitOther(this, stats, damageModifier);
    }

    public abstract class UsableItem : Item
    {
        protected int _equipSlotSize;
        public override int EquipSlotSize => _equipSlotSize;

        public UsableItem(string name, char symbol, int slots)
        {
            Name = name;
            Symbol = symbol;
            _equipSlotSize = slots;
        }

        public override bool EquipTo(EquipSlots hands) => hands.TryEquip(this, EquipSlotSize);
    }

    public abstract class Weapon : UsableItem
    {
        public int BaseDamage { get; protected set; }
        public abstract override int GetNoiseLevel();

        public Weapon(string name, char symbol, int damage, int slots) : base(name, symbol, slots)
        {
            BaseDamage = damage;
        }

        public override int GetBaseDamage() => BaseDamage;

        public override string GetInfo() => $"{Name} (DMG: {BaseDamage})";

        public override string GetInstruction() => $"Pick up weapon: {Name} (DMG: {BaseDamage})";
    }

    public abstract class HeavyWeapon : Weapon
    {
        public override int GetNoiseLevel() => 10;

        public HeavyWeapon(string name, char symbol, int damage, int slots) : base(name, symbol, damage, slots) { }
        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => attack.VisitHeavy(this, stats, damageModifier);
    }

    public abstract class LightWeapon : Weapon
    {
        public override int GetNoiseLevel() => 2;

        public LightWeapon(string name, char symbol, int damage, int slots) : base(name, symbol, damage, slots) { }
        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => attack.VisitLight(this, stats, damageModifier);
    }

    public abstract class MagicWeapon : Weapon
    {
        public override int GetNoiseLevel() => 5;

        public MagicWeapon(string name, char symbol, int damage, int slots) : base(name, symbol, damage, slots) { }
        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => attack.VisitMagic(this, stats, damageModifier);
    }

    public class LongSword : HeavyWeapon { public LongSword() : base("Long Sword", 'L', 20, 2) { } }
    public class ShortSword : LightWeapon { public ShortSword() : base("Short Sword", 's', 10, 1) { } }
    public class Dagger : LightWeapon { public Dagger() : base("Dagger", 'd', 5, 1) { } }
    public class MagicStaff : MagicWeapon { public MagicStaff() : base("Magic Staff", 'm', 15, 2) { } }
    public class Stick : Item { }

    public abstract class ItemDecorator : IItem
    {
        protected readonly IItem _innerItem;
        protected readonly string _modifierName;
        public virtual char Symbol => _innerItem.Symbol;

        public ItemDecorator(IItem innerItem, string modifierName)
        {
            _innerItem = innerItem;
            _modifierName = modifierName;
        }

        public IItem GetInnerItem() => _innerItem;

        public string Name => $"{_modifierName} {_innerItem.Name}";
        public int EquipSlotSize => _innerItem.EquipSlotSize;
        public bool IsStorable => _innerItem.IsStorable;

        public virtual int GetBaseDamage() => _innerItem.GetBaseDamage();
        public virtual int GetNoiseLevel() => _innerItem.GetNoiseLevel();
        public virtual Attributes ModifyAttributes(Attributes current) => _innerItem.ModifyAttributes(current);

        public virtual CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => _innerItem.Accept(attack, stats, damageModifier);

        public bool EquipTo(EquipSlots hands) => hands.TryEquip(this, EquipSlotSize);

        public void OnPickUp(Player player) => _innerItem.OnPickUp(player);

        public virtual string GetInfo() => $"{_modifierName} {_innerItem.GetInfo()}";
        public virtual string GetInstruction() => $"Pick up: {_modifierName} {_innerItem.Name}";
    }

    public class StrongModifier : ItemDecorator
    {
        public StrongModifier(IItem inner) : base(inner, "Strong") { }

        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => _innerItem.Accept(attack, stats, damageModifier + 5);

        public override string GetInfo() => $"{_innerItem.GetInfo()} [+5 DMG]";
    }

    public class WeakModifier : ItemDecorator
    {
        public WeakModifier(IItem inner) : base(inner, "Weak") { }

        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => _innerItem.Accept(attack, stats, damageModifier - 5);

        public override string GetInfo() => $"{_innerItem.GetInfo()} [-5 DMG]";
    }

    public class UnluckyModifier : ItemDecorator
    {
        public UnluckyModifier(IItem inner) : base(inner, "Unlucky") { }
        public override Attributes ModifyAttributes(Attributes current)
        {
            Attributes modified = _innerItem.ModifyAttributes(current);
            return new Attributes(modified.Dexterity, modified.Strength, modified.Luck - 5, modified.Aggression, modified.Wisdom);
        }
    }

    public class WisdomModifier : ItemDecorator
    {
        public WisdomModifier(IItem inner) : base(inner, "Wisdom") { }
        public override Attributes ModifyAttributes(Attributes current)
        {
            Attributes modified = _innerItem.ModifyAttributes(current);
            return new Attributes(modified.Dexterity, modified.Strength, modified.Luck, modified.Aggression, modified.Wisdom + 5);
        }
    }

    public class ArtifactModifier : ItemDecorator
    {
        public ArtifactModifier(IItem inner) : base(inner, "Legendary") { }

        public override char Symbol => '*';

        public override string GetInfo() => $"*** LEGENDARY {_innerItem.Name.ToUpper()} ***";

        public override CombatResult Accept(ICombatAttack attack, Attributes stats, int damageModifier = 0)
            => _innerItem.Accept(attack, stats, damageModifier + 25);
    }
}