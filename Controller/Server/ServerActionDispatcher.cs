using System;
using System.Collections.Generic;

namespace Project1.Network
{
    public class ServerActionDispatcher
    {
        private readonly Dictionary<string, Action<WorldInteractionHandler, Player>> _actions = new Dictionary<string, Action<WorldInteractionHandler, Player>>();

        public ServerActionDispatcher()
        {
            _actions["MoveUp"] = (w, p) => w.TryMove(0, -1);
            _actions["MoveDown"] = (w, p) => w.TryMove(0, 1);
            _actions["MoveLeft"] = (w, p) => w.TryMove(-1, 0);
            _actions["MoveRight"] = (w, p) => w.TryMove(1, 0);
            _actions["PickUp"] = (w, p) => w.PickUpItem();

            _actions["StanceNormal"] = (w, p) => { p.ActiveAttack = new NormalAttack(); p.ActiveStanceName = "Normal"; };
            _actions["StanceStealth"] = (w, p) => { p.ActiveAttack = new StealthAttack(); p.ActiveStanceName = "Stealth"; };
            _actions["StanceMagic"] = (w, p) => { p.ActiveAttack = new MagicAttack(); p.ActiveStanceName = "Magic"; };
        }

        public void Dispatch(string actionType, WorldInteractionHandler world, Player player)
        {
            if (_actions.TryGetValue(actionType, out var action))
            {
                action.Invoke(world, player);
            }
        }
    }
}