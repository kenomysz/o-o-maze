using System;
using System.Collections.Generic;

namespace Project1
{
    public class ItemFactory
    {
        private readonly Random _rng = new Random();
        private readonly List<Func<IItem>> _weaponBlueprints;
        private readonly List<Func<IItem>> _miscBlueprints;

        public ItemFactory(List<Func<IItem>> weaponBlueprints, List<Func<IItem>> miscBlueprints)
        {
            _weaponBlueprints = weaponBlueprints;
            _miscBlueprints = miscBlueprints;
        }

        public IItem CreateRandomWeapon()
        {
            int index = _rng.Next(_weaponBlueprints.Count);
            IItem weapon = _weaponBlueprints[index]();

            if (_rng.NextDouble() <= 0.5)
            {
                if (_rng.NextDouble() < 0.5) weapon = new StrongModifier(weapon);
                else if (_rng.NextDouble() < 0.5) weapon = new WeakModifier(weapon);

                if (_rng.NextDouble() < 0.5) weapon = new UnluckyModifier(weapon);
                else if (_rng.NextDouble() < 0.5) weapon = new WisdomModifier(weapon);
            }
            else
            {
                if (_rng.NextDouble() < 0.5) weapon = new UnluckyModifier(weapon);
                else if (_rng.NextDouble() < 0.5) weapon = new WisdomModifier(weapon);

                if (_rng.NextDouble() < 0.5) weapon = new StrongModifier(weapon);
                else if (_rng.NextDouble() < 0.5) weapon = new WeakModifier(weapon);
            }
            return weapon;
        }

        public IItem CreateRandomMiscItem()
        {
            int index = _rng.Next(_miscBlueprints.Count);
            return _miscBlueprints[index]();
        }
    }
}