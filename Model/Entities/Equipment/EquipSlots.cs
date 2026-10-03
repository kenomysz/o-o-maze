using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{


    public class EquipSlots
    {
        private readonly IItem[] _slots;

        public EquipSlots(int numberOfHands)
        {
            _slots = new IItem[numberOfHands];
        }

        public bool TryEquip(IItem item, int handsNeeded)
        {
            int freeCount = 0;
            foreach (var slot in _slots) if (slot == null) freeCount++;

            if (freeCount < handsNeeded) return false;

            int assigned = 0;
            for (int i = 0; i < _slots.Length && assigned < handsNeeded; i++)
            {
                if (_slots[i] == null)
                {
                    _slots[i] = item;
                    assigned++;
                }
            }

            return true;
        }

        public IItem RemoveItemFromHand(int index)
        {
            if (index < 0 || index >= _slots.Length || _slots[index] == null) return null;

            IItem itemToRemove = _slots[index];

            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == itemToRemove) _slots[i] = null;
            }

            return itemToRemove;
        }

        public IItem GetItemInHand(int index) => (index >= 0 && index < _slots.Length) ? _slots[index] : null;
    }
}
