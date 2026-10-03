using System;
using System.Collections.Generic;

namespace Project1
{
    public class Level : INoiseSubject
    {
        public Map Map { get; set; }
        public bool ContainsPickables { get; set; } = false;

        public EntityManager Entities { get; } = new EntityManager();

        private readonly List<INoiseObserver> _noiseObservers = new List<INoiseObserver>();
        private readonly List<ITurnParticipant> _turnParticipants = new List<ITurnParticipant>();

        public void Subscribe(INoiseObserver observer) => _noiseObservers.Add(observer);
        public void Unsubscribe(INoiseObserver observer) => _noiseObservers.Remove(observer);

        public void RegisterTurnParticipant(ITurnParticipant participant) => _turnParticipants.Add(participant);
        public void UnregisterTurnParticipant(ITurnParticipant participant) => _turnParticipants.Remove(participant);

        public void BroadcastSound(int startX, int startY, int maxRange, string sourceName)
        {
            var reachableCells = new Dictionary<(int x, int y), int>();
            var queue = new Queue<(int x, int y, int dist)>();

            queue.Enqueue((startX, startY, 0));
            reachableCells[(startX, startY)] = 0;

            (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.dist >= maxRange) continue;

                foreach (var dir in directions)
                {
                    int nx = current.x + dir.dx;
                    int ny = current.y + dir.dy;

                    if (Map.IsInBounds(nx, ny) && Map.GetCell(nx, ny).CanEnter() && !reachableCells.ContainsKey((nx, ny)))
                    {
                        reachableCells[(nx, ny)] = current.dist + 1;
                        queue.Enqueue((nx, ny, current.dist + 1));
                    }
                }
            }

            var currentObservers = new List<INoiseObserver>(_noiseObservers);
            foreach (var observer in currentObservers)
            {
                if (observer is Entity entity)
                {
                    if (reachableCells.TryGetValue((entity.Pos.X, entity.Pos.Y), out int actualDistance))
                    {
                        observer.OnSoundEmitted(startX, startY, actualDistance, sourceName);
                    }
                }
            }
        }
        public void UpdateWorld(Random rng)
        {
            var participants = new List<ITurnParticipant>(_turnParticipants);
            foreach (var participant in participants)
            {
                participant.UpdateTurn(rng);
            }
        }


    }
}