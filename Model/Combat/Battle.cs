using System;
using System.Collections.Generic;

namespace Project1
{
    public class Battle
    {
        public Enemy TargetEnemy { get; }
        public List<Character> Participants { get; } = new List<Character>();
        public int CurrentTurnIndex { get; private set; } = 0;
        public bool IsFinished { get; set; } = false;
        public string LastActionMessage { get; set; } = string.Empty;

        public Character CurrentActor => Participants.Count > 0 ? Participants[CurrentTurnIndex] : null;

        public Battle(Enemy enemy)
        {
            TargetEnemy = enemy;
            Participants.Add(enemy); 
        }

        public void Join(Player player)
        {
            if (!Participants.Contains(player))
            {

                int enemyIndex = Participants.IndexOf(TargetEnemy);
                Participants.Insert(enemyIndex, player);
                if (CurrentTurnIndex >= enemyIndex) CurrentTurnIndex++;
            }
        }

        public void Leave(Player player)
        {
            int index = Participants.IndexOf(player);
            if (index >= 0)
            {
                Participants.RemoveAt(index);
                if (CurrentTurnIndex > index) CurrentTurnIndex--;
                if (CurrentTurnIndex >= Participants.Count) CurrentTurnIndex = 0;
            }
            if (Participants.Count <= 1) IsFinished = true; 
        }

        public void NextTurn()
        {
            if (IsFinished || Participants.Count == 0) return;

            
            do
            {
                CurrentTurnIndex = (CurrentTurnIndex + 1) % Participants.Count;
            }
            while (CurrentActor.IsDead && !IsFinished);
        }
    }
}