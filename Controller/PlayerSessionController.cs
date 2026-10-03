using System.Collections.Generic;

namespace Project1.Network
{
    public class PlayerSessionController
    {
        public Player Player { get; }
        public WorldInteractionHandler WorldHandler { get; }
        public CombatController CombatController { get; }
        public InventoryController InventoryController { get; }
        public List<string> IntroText { get; }
        public string SystemMessage { get; set; } = string.Empty;

        public SessionState CurrentState { get; private set; }
        public string CurrentMenu => CurrentState.Name;
        public int InventoryIndex => InventoryController.SelectedIndex;

        public PlayerSessionController(Player player, WorldInteractionHandler worldHandler, CombatController combatController, List<string> introText)
        {
            Player = player;
            WorldHandler = worldHandler;
            CombatController = combatController;
            IntroText = introText;

            InventoryController = new InventoryController(Player, WorldHandler);
            ChangeState(new IntroState(this));
        }

        public void ChangeState(SessionState newState)
        {
            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState.OnEnter();
        }

        public char GetPlayerSymbol() => Player.Symbol;

        public void EnterCombat(Enemy enemy)
        {

            var battle = CombatController.GetOrStartBattle(Player, enemy);
            ChangeState(new CombatState(this, battle));
        }

        public void HandleClientAction(string actionType)
        {
            if (Player.IsDead && CurrentMenu != "Death")
            {
                ChangeState(new DeathState(this));
                return;
            }
            if (Player.IsDead) return;

            SystemMessage = string.Empty;

            if (actionType.StartsWith("LogError_"))
            {
                string errorMessage = actionType.Substring(9);
                SystemMessage = errorMessage;
                Journal.Instance.Log(new InputLog($"[{Player.Name}] {errorMessage}"));
                return;
            }

            CurrentState.HandleAction(actionType);
        }

        public List<string> GetPlayerInventoryInfo()
        {
            List<string> info = new List<string>();
            foreach (var item in Player.Inventory) info.Add(item.GetInfo());
            return info;
        }
    }
}