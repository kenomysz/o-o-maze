using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1.Network
{
    public class GameEngine
    {
        public Level Level { get; }
        public CombatController CombatController { get; }
        public object StateLock { get; } = new object();

        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private readonly Dictionary<int, PlayerSessionController> _sessions = new Dictionary<int, PlayerSessionController>();
        private readonly List<string> _dungeonManual;

        public GameEngine(Level level, List<string> dungeonManual)
        {
            Level = level;
            _dungeonManual = dungeonManual;
            CombatController = new CombatController(Level);
        }

        public void SpawnPlayer(int playerId)
        {
            var player = new Player(5, 5, $"Player {playerId}", playerId.ToString()[0]);
            _players[playerId] = player;
            Level.Entities.Add(player);
            Level.Subscribe(player);

            var interactionRoot = new BoundsHandler();
            interactionRoot.SetNext(new WallHandler())
                           .SetNext(new CombatHandler(this)) 
                           .SetNext(new MoveHandler())
                           .SetNext(new PickUpHandler())
                           .SetNext(new DropHandler());

            var worldHandler = new WorldInteractionHandler(Level, player, interactionRoot);

            _sessions[playerId] = new PlayerSessionController(player, worldHandler, CombatController, _dungeonManual);

            Journal.Instance.Log(new SystemLog($"Player {playerId} has connected to the server."));
        }

        public void RemovePlayer(int playerId)
        {
            if (_players.TryGetValue(playerId, out Player player))
            {
                Level.Entities.Remove(player);
                Level.Unsubscribe(player);
                _players.Remove(playerId);
            }
            _sessions.Remove(playerId);
        }

        public PlayerSessionController GetSessionForPlayer(char symbol)
        {
            foreach (var session in _sessions.Values)
            {
                if (session.GetPlayerSymbol() == symbol) return session;
            }
            return null;
        }

        public void ProcessAction(int playerId, ClientActionDTO action)
        {
            if (_sessions.TryGetValue(playerId, out var session))
            {
                session.HandleClientAction(action.ActionType);
            }
        }

        public void Tick(Random rng)
        {
            Level.UpdateWorld(rng);
            CombatController.UpdateAITurns(this);

            foreach (var player in _players.Values.ToList())
            {
                if (player.IsDead && Level.Entities.GetAll().Contains(player))
                {
                    Level.Entities.Remove(player);
                    Journal.Instance.Log(new SystemLog($"{player.Name} vanished from the realm."));
                }
            }
        }

        public GameStateDTO BuildGameStateDTO(int targetPlayerId)
        {
            var dto = new GameStateDTO
            {
                MapWidth = Level.Map.Width,
                MapHeight = Level.Map.Height,
            };

            for (int x = 0; x < Level.Map.Width; x++)
            {
                for (int y = 0; y < Level.Map.Height; y++)
                {
                    var cell = Level.Map.GetCell(x, y);
                    dto.Cells.Add(new CellDTO { X = x, Y = y, Symbol = cell.Symbol });
                }
            }

            foreach (var entity in Level.Entities.GetAll())
            {
                var edto = new EntityDTO
                {
                    X = entity.Pos.X,
                    Y = entity.Pos.Y,
                    Symbol = entity.Symbol,
                    Name = entity.Name,
                    Hp = entity.HP.Health,
                    MaxHp = entity.HP.MaxHealth,
                    IsPlayer = entity.IsControlledByPlayer
                };

                if (entity is Player p) edto.Stance = p.ActiveStanceName;
                dto.Entities.Add(edto);
            }

            if (_sessions.TryGetValue(targetPlayerId, out var session))
            {
                dto.MyUI.CurrentMenu = session.CurrentMenu;
                dto.MyUI.MenuSelectedIndex = session.InventoryIndex;
                dto.MyUI.InventoryItems = session.GetPlayerInventoryInfo();
                dto.MyUI.SystemMessage = session.SystemMessage;

                if (session.CurrentMenu == "Intro") dto.MyUI.IntroText = session.IntroText;

                if (_players.TryGetValue(targetPlayerId, out Player p))
                {
                    var left = p.Hands.GetItemInHand(0);
                    var right = p.Hands.GetItemInHand(1);
                    dto.MyUI.LeftHand = left != null ? left.GetInfo() : "Empty";
                    dto.MyUI.RightHand = right != null ? right.GetInfo() : "Empty";
                }

                if (session.CurrentState is CombatState combatState && combatState.Battle.TargetEnemy != null)
                {
                    dto.MyUI.EnemyName = combatState.Battle.TargetEnemy.Name;
                    dto.MyUI.EnemyHp = combatState.Battle.TargetEnemy.HP.Health;
                    dto.MyUI.EnemyMaxHp = combatState.Battle.TargetEnemy.HP.MaxHealth;
                    dto.MyUI.EnemyAtk = combatState.Battle.TargetEnemy.AttackPower;
                    dto.MyUI.EnemyArm = combatState.Battle.TargetEnemy.Armor;
                    dto.MyUI.IsMyTurn = combatState.Battle.CurrentActor == session.Player;

                    if (string.IsNullOrEmpty(dto.MyUI.SystemMessage) && !string.IsNullOrEmpty(combatState.Battle.LastActionMessage))
                    {
                        dto.MyUI.SystemMessage = combatState.Battle.LastActionMessage;
                    }
                }
            }
            return dto;
        }
    }
}