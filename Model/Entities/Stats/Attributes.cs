using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public class Attributes
    {
        public int Dexterity { get; private set; }
        public int Strength { get; private set; }
        public int Luck { get; private set; }
        public int Aggression { get; private set; }
        public int Wisdom { get; private set; }

        public Attributes(int dexterity, int strength, int luck, int aggression, int wisdom)
        {
            Dexterity = dexterity;
            Strength = strength;
            Luck = luck;
            Aggression = aggression;
            Wisdom = wisdom;
        }

        public Attributes() { }
        public IEnumerable<(string Name, int Value)> GetAllAttributes()
        {
            yield return ("Strength", Strength);
            yield return ("Dexterity", Dexterity);
            yield return ("Wisdom", Wisdom);
            yield return ("Aggression", Aggression);
            yield return ("Luck", Luck);
        }
    }
}
