
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    public interface INoiseObserver
    {
        void OnSoundEmitted(int sourceX, int sourceY, int actualDistance, string sourceName);
    }

    public interface INoiseSubject
    {
        void Subscribe(INoiseObserver observer);
        void Unsubscribe(INoiseObserver observer);
        void BroadcastSound(int x, int y, int range, string sourceName);
    }

    public interface ISpeciesObserver
    {
        void OnAllyDied();
    }

    public interface ISpeciesSubject
    {
        void Attach(ISpeciesObserver observer);
        void Detach(ISpeciesObserver observer);
        void NotifyAllyDeath(ISpeciesObserver deceased);
    }

    public interface ITurnParticipant
    {
        void UpdateTurn(System.Random rng);
    }
}