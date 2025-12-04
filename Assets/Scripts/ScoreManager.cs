using System.Collections.Generic;
using UnityEngine;

namespace CarnivalShooter2D.Observer
{
    public class ScoreManager : MonoBehaviour, IScoreSubject
    {
        public static ScoreManager Instance { get; private set; }

        [HideInInspector] public int score = 0;
        readonly List<IScoreObserver> observers = new();

        void Awake() => Instance = this;

        public void AddPoints(int points)
        {
            score += points;
            Notify(score);
        }

        public void Register(IScoreObserver obs)
        {
            if (!observers.Contains(obs)) observers.Add(obs);
            obs.OnScoreChanged(score);
        }

        public void Unregister(IScoreObserver obs) => observers.Remove(obs);

        public void Notify(int newScore)
        {
            foreach (var o in observers) o.OnScoreChanged(newScore);
        }
    }
}
