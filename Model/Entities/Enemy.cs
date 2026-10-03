using System;

namespace Project1
{
    public class Enemy : Character, INoiseObserver, ISpeciesObserver, ITurnParticipant
    {
        public int AttackPower { get; set; }
        public int Armor { get; set; }



        private int _engagedPlayersCount = 0;
        public bool IsEngagedInCombat => _engagedPlayersCount > 0;
        public override bool IsControlledByPlayer => false;

        public override CollisionResult HandleCollision(Player movingPlayer)
        {
            if (!IsDead)
            {
                return CollisionResult.StartCombat;
            }
            return CollisionResult.Allowed;
        }
        public override string Name
        {
            get
            {
                if (_isEffectActive)
                {
                    return $"{base.Name} {_faction.ReactionStrategy.StatusName}";
                }
                return base.Name;
            }
        }

        protected Faction _faction;
        protected Level _level;

        private readonly IEnemyMovementStrategy _movementStrategy;

        private int _effectTurnsLeft = 0;
        private bool _isEffectActive = false;
        private const int EffectDuration = 10;
        public Enemy(int x, int y, string name, char symbol, int hp, int attack, int armor, Attributes stats, Faction faction, Level level, IEnemyMovementStrategy movementStrategy)
            : base(x, y, name, symbol)
        {
            HP = new HealthBar(hp, hp);
            AttackPower = attack;
            Armor = armor;
            Stats = stats;

            _faction = faction;
            _level = level;
            _movementStrategy = movementStrategy;

            _faction.Attach(this);
            _level.Subscribe(this);
            _level.RegisterTurnParticipant(this);
        }

        public void Engage() => _engagedPlayersCount++;
        public void Disengage()
        {
            if (_engagedPlayersCount > 0) _engagedPlayersCount--;
        }
        public void OnAllyDied()
        {
            if (!_isEffectActive)
            {
                _faction.ReactionStrategy.ApplyEffect(this);
                _isEffectActive = true;
            }
            _effectTurnsLeft = EffectDuration;
        }

        public void OnSoundEmitted(int sourceX, int sourceY, int actualDistance, string sourceName)
        {
            Journal.Instance.Log(new GameLog($"[{Name} at {Pos.X},{Pos.Y}] heard noise '{sourceName}' from distance {actualDistance}."));
        }

        public void UpdateTurn(Random rng)
        {
            if (IsDead) return;

            if (_isEffectActive)
            {
                _effectTurnsLeft--;
                if (_effectTurnsLeft <= 0)
                {
                    _faction.ReactionStrategy.RevertEffect(this);
                    _isEffectActive = false;
                }
            }

            if (!IsEngagedInCombat)
            {
                _movementStrategy.ExecuteMove(this, _level, rng);
            }
        }

        public override void Die()
        {
            base.Die();

            _faction.NotifyAllyDeath(this);
            _faction.Detach(this);
            _level.Unsubscribe(this);
            _level.UnregisterTurnParticipant(this);
        }
        public bool IsEffectActive => _isEffectActive;
        public string ActiveEffectName => _isEffectActive ? _faction.ReactionStrategy.StatusName : string.Empty;

        public string DisplayName => _isEffectActive ? $"{Name} {ActiveEffectName}" : Name;
    }
}