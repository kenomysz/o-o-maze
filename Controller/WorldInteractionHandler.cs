using System;
using Project1.Network;

namespace Project1
{
    public enum InteractionType
    {
        Move,
        PickUp,
        Drop
    }

    public class InteractionRequest
    {
        public InteractionType Type { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public Player Player { get; set; }
        public Level Level { get; set; }
        public IItem Payload { get; set; }
        public string ResultMessage { get; set; } = string.Empty;
    }

    public abstract class BaseInteractionHandler
    {
        private BaseInteractionHandler _next;

        public BaseInteractionHandler SetNext(BaseInteractionHandler next)
        {
            _next = next;
            return next;
        }

        public virtual void Handle(InteractionRequest request)
        {
            _next?.Handle(request);
        }
    }

    public class BoundsHandler : BaseInteractionHandler
    {
        public override void Handle(InteractionRequest request)
        {
            if (!request.Level.Map.IsInBounds(request.TargetX, request.TargetY))
            {
                request.ResultMessage = "You cannot go that way.";
                return;
            }
            base.Handle(request);
        }
    }

    public class WallHandler : BaseInteractionHandler
    {
        public override void Handle(InteractionRequest request)
        {
            if (request.Type == InteractionType.Move)
            {
                Cell targetCell = request.Level.Map.GetCell(request.TargetX, request.TargetY);
                if (targetCell != null && !targetCell.CanEnter())
                {
                    request.ResultMessage = "You bump into a solid wall.";
                    Journal.Instance.Log(new GameLog($"{request.Player.Name} bumps into a wall at ({request.TargetX}, {request.TargetY})."));
                    return;
                }
            }
            base.Handle(request);
        }
    }



    public class CombatHandler : BaseInteractionHandler
    {
        private readonly Project1.Network.GameEngine _engine;

        public CombatHandler(Project1.Network.GameEngine engine)
        {
            _engine = engine;
        }

        public override void Handle(InteractionRequest request)
        {
            if (request.Type == InteractionType.Move)
            {
                Character occupant = request.Level.Entities.GetAt(request.TargetX, request.TargetY);

                if (occupant != null)
                {
                    CollisionResult collisionResult = occupant.HandleCollision(request.Player);

                    if (collisionResult == CollisionResult.StartCombat && occupant is Enemy enemy)
                    {
                        var session = _engine.GetSessionForPlayer(request.Player.Symbol);
                        if (session != null)
                        {
                            session.EnterCombat(enemy);
                            request.ResultMessage = $"You encountered {occupant.Name}!";
                            return;
                        }
                    }
                    else if (collisionResult == CollisionResult.Blocked)
                    {
                        request.ResultMessage = $"The path is blocked by {occupant.Name}.";
                        return;
                    }
                }
            }
            base.Handle(request);
        }
    }
        public class MoveHandler : BaseInteractionHandler
    {
        public override void Handle(InteractionRequest request)
        {
            if (request.Type == InteractionType.Move)
            {
                request.Level.Entities.UpdatePosition(request.Player, request.TargetX, request.TargetY);
                request.Player.Move(request.TargetX, request.TargetY);
                return;
            }
            base.Handle(request);
        }
    }

    public class PickUpHandler : BaseInteractionHandler
    {
        public override void Handle(InteractionRequest request)
        {
            if (request.Type == InteractionType.PickUp)
            {
                Cell currentCell = request.Level.Map.GetCell(request.TargetX, request.TargetY);
                if (currentCell != null && currentCell.HasItems())
                {
                    IItem itemToPickUp = currentCell.TakeTopItem();

                    itemToPickUp.OnPickUp(request.Player);

                    if (itemToPickUp.IsStorable)
                    {
                        request.Player.AddToInventory(itemToPickUp);
                        request.ResultMessage = $"Picked up: {itemToPickUp.Name}";
                        Journal.Instance.Log(new GameLog($"{request.Player.Name} picks up item: {itemToPickUp.Name}."));

                        int noise = itemToPickUp.GetNoiseLevel();
                        if (noise > 0)
                        {
                            request.Level.BroadcastSound(request.TargetX, request.TargetY, noise, $"Pick up: {itemToPickUp.Name}");
                        }
                    }
                }
                return;
            }
            base.Handle(request);
        }
    }

    public class DropHandler : BaseInteractionHandler
    {
        public override void Handle(InteractionRequest request)
        {
            if (request.Type == InteractionType.Drop)
            {
                if (request.Payload != null)
                {
                    Cell currentCell = request.Level.Map.GetCell(request.TargetX, request.TargetY);
                    if (currentCell != null)
                    {
                        currentCell.AddItem(request.Payload);
                        Journal.Instance.Log(new GameLog($"{request.Player.Name} drops item: {request.Payload.Name}."));

                        int noise = request.Payload.GetNoiseLevel();
                        if (noise > 0)
                        {
                            request.Level.BroadcastSound(request.TargetX, request.TargetY, noise, $"Drop: {request.Payload.Name}");
                        }
                    }
                }
                return;
            }
            base.Handle(request);
        }
    }

    public class WorldInteractionHandler
    {
        private readonly Level _level;
        private readonly Player _player;
        private readonly BaseInteractionHandler _interactionChain;

        public WorldInteractionHandler(Level level, Player player, BaseInteractionHandler interactionChain)
        {
            _level = level;
            _player = player;
            _interactionChain = interactionChain;
        }

        public string TryMove(int dx, int dy)
        {
            var request = new InteractionRequest
            {
                Type = InteractionType.Move,
                TargetX = _player.Pos.X + dx,
                TargetY = _player.Pos.Y + dy,
                Player = _player,
                Level = _level
            };

            _interactionChain.Handle(request);


            return request.ResultMessage;
        }

        public bool CanPickUpItem()
        {
            Cell currentCell = _level.Map.GetCell(_player.Pos.X, _player.Pos.Y);
            return currentCell != null && currentCell.HasItems();
        }

        public string PickUpItem()
        {
            var request = new InteractionRequest
            {
                Type = InteractionType.PickUp,
                TargetX = _player.Pos.X,
                TargetY = _player.Pos.Y,
                Player = _player,
                Level = _level
            };

            _interactionChain.Handle(request);


            return request.ResultMessage;
        }

        public string DropItem(IItem item)
        {
            var request = new InteractionRequest
            {
                Type = InteractionType.Drop,
                TargetX = _player.Pos.X,
                TargetY = _player.Pos.Y,
                Player = _player,
                Level = _level,
                Payload = item
            };

            _interactionChain.Handle(request);


            return request.ResultMessage;
        }
    }
}