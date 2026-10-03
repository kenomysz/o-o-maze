using System;
using System.Collections.Generic;
using System.Linq;
using Project1.Network;

namespace Project1
{
    public class CombatController
    {
        private readonly Level _level;
        public List<Battle> ActiveBattles { get; } = new List<Battle>();

        public CombatController(Level level) { _level = level; }

        public Battle GetOrStartBattle(Player player, Enemy enemy)
        {
            var battle = ActiveBattles.FirstOrDefault(b => b.TargetEnemy == enemy);
            if (battle == null)
            {
                battle = new Battle(enemy);
                ActiveBattles.Add(battle);
            }
            battle.Join(player);
            return battle;
        }

        public void ExecutePlayerAttack(Battle battle, Player player, ICombatAttack chosenAttack)
        {
            if (battle.CurrentActor != player || battle.IsFinished) return;

            Attributes combatStats = player.EffectiveStats;
            var equippedItems = player.GetEquippedItems().Distinct().ToList();

            int totalDamage = 0;
            if (!equippedItems.Any()) totalDamage = new Stick().Accept(chosenAttack, combatStats).Damage;
            else foreach (var item in equippedItems) totalDamage = Math.Max(totalDamage, item.Accept(chosenAttack, combatStats).Damage);

            int finalEnemyDamage = Math.Max(0, totalDamage - battle.TargetEnemy.Armor);
            battle.TargetEnemy.TakeDamage(finalEnemyDamage);

            string msg = $"{player.Name} attacks {battle.TargetEnemy.Name} for {finalEnemyDamage} DMG.";
            Journal.Instance.Log(new GameLog(msg));
            battle.LastActionMessage = msg;

            if (battle.TargetEnemy.IsDead)
            {
                _level.Entities.Remove(battle.TargetEnemy);

                string winMsg = $"Enemy {battle.TargetEnemy.Name} has been defeated!";
                Journal.Instance.Log(new GameLog(winMsg));
                battle.LastActionMessage = winMsg;

                battle.IsFinished = true;
            }
            else
            {
                battle.NextTurn(); 
            }
        }

        public void UpdateAITurns(Project1.Network.GameEngine engine)
        {
            foreach (var battle in ActiveBattles.ToList())
            {
                if (battle.IsFinished)
                {
                    EndBattle(battle, engine);
                    continue;
                }

                if (battle.CurrentActor == battle.TargetEnemy)
                {
                    var alivePlayers = battle.Participants.OfType<Player>().Where(p => !p.IsDead).ToList();
                    if (!alivePlayers.Any())
                    {
                        battle.IsFinished = true;
                        EndBattle(battle, engine);
                        continue;
                    }

                    var targetPlayer = alivePlayers[new Random().Next(alivePlayers.Count)];

                    int defense = 0;
                    foreach (var item in targetPlayer.GetEquippedItems()) defense += item.Accept(new NormalAttack(), targetPlayer.EffectiveStats).Defense;

                    int finalPlayerDamage = Math.Max(0, battle.TargetEnemy.AttackPower - defense);
                    targetPlayer.TakeDamage(finalPlayerDamage);

                    string msg = $"{battle.TargetEnemy.Name} attacks {targetPlayer.Name} for {finalPlayerDamage} DMG.";
                    Journal.Instance.Log(new GameLog(msg));
                    battle.LastActionMessage = msg;

                    if (targetPlayer.IsDead)
                    {
                        string deathMsg = $"{targetPlayer.Name} has been slain.";
                        Journal.Instance.Log(new GameLog(deathMsg));
                        battle.LastActionMessage = deathMsg;

                        battle.Leave(targetPlayer);
                        var session = engine.GetSessionForPlayer(targetPlayer.Symbol);
                        if (session != null) session.ChangeState(new DeathState(session));
                    }

                    if (!battle.IsFinished) battle.NextTurn();
                }
            }
        }

        private void EndBattle(Battle battle, GameEngine engine)
        {
            ActiveBattles.Remove(battle);
            foreach (var p in battle.Participants.OfType<Player>())
            {
                var session = engine.GetSessionForPlayer(p.Symbol);
                if (session != null && session.CurrentState is CombatState) session.ChangeState(new ExplorationState(session));
            }
        }
    }
}