using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public abstract class Entity : IDrawable
    {
        private Position _pos;
        public Position Pos => _pos;
        public virtual string Name { get; protected set; } = "";

        public char Symbol { get; protected set; }

        public Entity(int startX, int startY, string name, char symbol)
        {
            _pos = new Position(startX, startY);
            Name = name;
            Symbol = symbol;
        }

        public virtual void Move(int newX, int newY)
        {
            _pos.X = newX;
            _pos.Y = newY;
        }
    }
}