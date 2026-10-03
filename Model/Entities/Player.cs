using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public class Player : Character, INoiseObserver
    {
        private List<IItem> _inventory = new List<IItem>();
        public IReadOnlyList<IItem> Inventory => _inventory;

        public EquipSlots Hands { get; } = new EquipSlots(2);

        public override bool IsControlledByPlayer => true;

        public override CollisionResult HandleCollision(Player movingPlayer)
        {
            return CollisionResult.Allowed;
        }

        public ICombatAttack ActiveAttack { get; set; } = new NormalAttack();
        public string ActiveStanceName { get; set; } = "Normal";

        public int Gold { get; private set; }
        public int Coins { get; private set; }

        public List<string> PendingSounds { get; } = new List<string>();

        public void OnSoundEmitted(int startX, int startY, int maxRange, string sourceName)
        {
            if (startX == Pos.X && startY == Pos.Y) return;

            int dist = Math.Abs(Pos.X - startX) + Math.Abs(Pos.Y - startY);
            if (dist <= maxRange)
            {
                PendingSounds.Add($"You hear '{sourceName}' nearby.");
            }
            Journal.Instance.Log(new GameLog($"[{Name} at {Pos.X},{Pos.Y}] heard noise '{sourceName}'."));
        }

        public Attributes EffectiveStats
        {
            get
            {
                Attributes current = Stats;
                foreach (var item in GetEquippedItems().Distinct())
                {
                    current = item.ModifyAttributes(current);
                }
                return current;
            }
        }

        public Player(int startX, int startY, string name, char symbol = '¶') : base(startX, startY, name, symbol)
        {
        }

        public void AddGold(int amount) { Gold += amount; }
        public void AddCoins(int amount) { Coins += amount; }
        public void AddToInventory(IItem item) { _inventory.Add(item); }

        public override IEnumerable<IItem> GetEquippedItems()
        {
            if (Hands.GetItemInHand(0) != null) yield return Hands.GetItemInHand(0);
            if (Hands.GetItemInHand(1) != null) yield return Hands.GetItemInHand(1);
        }

        public IItem RemoveFromInventory(int index)
        {
            if (index >= 0 && index < _inventory.Count)
            {
                IItem item = _inventory[index];
                _inventory.RemoveAt(index);
                return item;
            }
            return null;
        }

        public bool EquipFromInventory(int inventoryIndex)
        {
            if (inventoryIndex < 0 || inventoryIndex >= _inventory.Count) return false;

            IItem item = _inventory[inventoryIndex];

            if (item.EquipTo(this.Hands))
            {
                _inventory.RemoveAt(inventoryIndex);
                return true;
            }
            return false;
        }

        public void UnequipToInventory(int slotIndex)
        {
            IItem item = Hands.RemoveItemFromHand(slotIndex);
            if (item != null) _inventory.Add(item);
        }

        public IItem DropFromHand(int slotIndex) => Hands.RemoveItemFromHand(slotIndex);
    }
}