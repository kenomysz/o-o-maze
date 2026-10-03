using System;
using System.Collections.Generic;

namespace Project1.Network
{
    public abstract class SessionState
    {
        protected PlayerSessionController Ctx { get; }
        public abstract string Name { get; }

        protected SessionState(PlayerSessionController ctx)
        {
            Ctx = ctx;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public abstract void HandleAction(string actionType);

        protected void LogUnavailableAction()
        {
            Ctx.SystemMessage = $"Action unavailable in {Name} mode.";
            Journal.Instance.Log(new InputLog($"[{Ctx.Player.Name}] Tried to use unavailable action in {Name} mode."));
        }
    }

    public class IntroState : SessionState
    {
        public override string Name => "Intro";
        public IntroState(PlayerSessionController ctx) : base(ctx) { }

        public override void HandleAction(string actionType)
        {
            if (actionType == "Action_Start") Ctx.ChangeState(new ExplorationState(Ctx));
        }
    }

    public class ExplorationState : SessionState
    {
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        public override string Name => "Exploration";

        public ExplorationState(PlayerSessionController ctx) : base(ctx)
        {
            _actions["MoveUp"] = () => Ctx.WorldHandler.TryMove(0, -1);
            _actions["MoveDown"] = () => Ctx.WorldHandler.TryMove(0, 1);
            _actions["MoveLeft"] = () => Ctx.WorldHandler.TryMove(-1, 0);
            _actions["MoveRight"] = () => Ctx.WorldHandler.TryMove(1, 0);

            _actions["Action_E"] = () => Ctx.WorldHandler.PickUpItem();
            _actions["Action_I"] = () =>
            {
                Ctx.InventoryController.ResetSelection();
                Ctx.ChangeState(new InventoryState(Ctx));
            };

            _actions["Action_1"] = () => { Ctx.Player.ActiveAttack = new NormalAttack(); Ctx.Player.ActiveStanceName = "Normal"; };
            _actions["Action_2"] = () => { Ctx.Player.ActiveAttack = new StealthAttack(); Ctx.Player.ActiveStanceName = "Stealth"; };
            _actions["Action_3"] = () => { Ctx.Player.ActiveAttack = new MagicAttack(); Ctx.Player.ActiveStanceName = "Magic"; };
        }

        public override void HandleAction(string actionType)
        {
            if (_actions.TryGetValue(actionType, out var action)) action.Invoke();
            else if (actionType.StartsWith("Action_")) LogUnavailableAction();
        }
    }

    public class InventoryState : SessionState
    {
        private readonly Dictionary<string, Action> _actions = new Dictionary<string, Action>();
        public override string Name => "Inventory";

        public InventoryState(PlayerSessionController ctx) : base(ctx)
        {
            _actions["MoveUp"] = () => Ctx.InventoryController.MoveSelectionUp();
            _actions["MoveDown"] = () => Ctx.InventoryController.MoveSelectionDown();

            _actions["Action_I"] = () => Ctx.ChangeState(new ExplorationState(Ctx));
            _actions["Action_ESC"] = () => Ctx.ChangeState(new ExplorationState(Ctx));

            _actions["Action_1"] = () => { Ctx.SystemMessage = Ctx.InventoryController.UnequipFromSpecificHand(0); };
            _actions["Action_2"] = () => { Ctx.SystemMessage = Ctx.InventoryController.UnequipFromSpecificHand(1); };

            _actions["Action_E"] = () =>
            {
                string result = Ctx.InventoryController.EquipSelected();
                Journal.Instance.Log(new GameLog($"[{Ctx.Player.Name}] {result}"));
            };

            _actions["Action_Q"] = () =>
            {
                string result = Ctx.InventoryController.DropSelected();
                Journal.Instance.Log(new GameLog($"[{Ctx.Player.Name}] {result}"));
            };
        }

        public override void HandleAction(string actionType)
        {
            if (_actions.TryGetValue(actionType, out var action)) action.Invoke();
            else if (actionType.StartsWith("Action_")) LogUnavailableAction();
        }
    }

    public class CombatState : SessionState
    {
        public override string Name => "Combat";
        public Battle Battle { get; }

        public CombatState(PlayerSessionController ctx, Battle battle) : base(ctx)
        {
            Battle = battle;
        }

        public override void HandleAction(string actionType)
        {
            if (Battle.IsFinished) return;

            if (actionType == "Action_ESC")
            {
                Journal.Instance.Log(new GameLog($"{Ctx.Player.Name} fled from combat!"));
                Battle.Leave(Ctx.Player);
                Ctx.ChangeState(new ExplorationState(Ctx));
                return;
            }

            if (Battle.CurrentActor != Ctx.Player)
            {
                Ctx.SystemMessage = "Wait for your turn in the battle queue!";
                return;
            }

            if (actionType == "Action_1") Ctx.CombatController.ExecutePlayerAttack(Battle, Ctx.Player, new NormalAttack());
            else if (actionType == "Action_2") Ctx.CombatController.ExecutePlayerAttack(Battle, Ctx.Player, new StealthAttack());
            else if (actionType == "Action_3") Ctx.CombatController.ExecutePlayerAttack(Battle, Ctx.Player, new MagicAttack());
            else if (actionType.StartsWith("Action_")) LogUnavailableAction();
        }
    }

    public class DeathState : SessionState
    {
        public override string Name => "Death";
        public DeathState(PlayerSessionController ctx) : base(ctx) { }

        public override void HandleAction(string actionType)
        {
            // smierc
        }
    }
}