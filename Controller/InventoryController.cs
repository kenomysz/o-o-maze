using System;

namespace Project1
{
    public class InventoryController
    {
        private readonly Player _player;
        private readonly WorldInteractionHandler _worldHandler;

        public int SelectedIndex { get; private set; } = 0;

        public InventoryController(Player player, WorldInteractionHandler worldHandler)
        {
            _player = player;
            _worldHandler = worldHandler;
        }

        public int GetInventoryCount() => _player.Inventory.Count;

        public bool HasAnythingEquipped()
        {
            return _player.Hands.GetItemInHand(0) != null || _player.Hands.GetItemInHand(1) != null;
        }

        public void MoveSelectionUp()
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
        }

        public void MoveSelectionDown()
        {
            SelectedIndex = Math.Min(GetInventoryCount() - 1, SelectedIndex + 1);
        }

        public string DropSelected()
        {
            if (GetInventoryCount() == 0) return "Inventory is empty.";

            IItem item = _player.RemoveFromInventory(SelectedIndex);
            _worldHandler.DropItem(item);

            AdjustStateAfterAction();
            return $"Dropped: {item.Name}";
        }

        public string EquipSelected()
        {
            if (GetInventoryCount() == 0) return "Inventory is empty.";

            IItem itemToEquip = _player.Inventory[SelectedIndex];
            bool equipped = _player.EquipFromInventory(SelectedIndex);

            if (!equipped)
            {
                return $"Cannot equip: {itemToEquip.Name}. (Not equippable or hands full)";
            }

            AdjustStateAfterAction();
            return $"Equipped: {itemToEquip.Name}.";
        }

        public string UnequipFromSpecificHand(int handIndex)
        {
            IItem currentlyEquipped = _player.Hands.GetItemInHand(handIndex);

            if (currentlyEquipped == null)
            {
                return $"Hand {handIndex + 1} is empty.";
            }

            _player.UnequipToInventory(handIndex);

            return $"Unequipped {currentlyEquipped.Name} from hand {handIndex + 1} to inventory.";
        }

        private void AdjustStateAfterAction()
        {
            if (GetInventoryCount() == 0)
            {
                SelectedIndex = 0;
            }
            else if (SelectedIndex >= GetInventoryCount())
            {
                SelectedIndex = GetInventoryCount() - 1;
            }
        }
        public void ResetSelection()
        {
            SelectedIndex = 0;
        }
    }
}