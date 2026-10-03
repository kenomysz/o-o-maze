using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{

    public abstract class Currency : Item
    {
        public override bool IsStorable => false;
        public int Value { get; protected set; }
        public Currency(int value) { Value = value; }
        public override string GetInstruction() => $"Zbierz: {Name} ({Value})";
    }

    public class Gold : Currency
    {
        public Gold(int value) : base(value) { Name = "Gold"; Symbol = '$'; }
        public override void OnPickUp(Player player) => player.AddGold(Value);
    }

    public class Coins : Currency
    {
        public Coins(int value) : base(value) { Name = "Coins"; Symbol = 'c'; }
        public override void OnPickUp(Player player) => player.AddCoins(Value);
    }

}
