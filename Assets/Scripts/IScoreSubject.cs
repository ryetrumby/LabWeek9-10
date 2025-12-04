namespace CarnivalShooter2D.Observer
{
    public interface IScoreSubject
    {
        void Register(IScoreObserver obs);
        void Unregister(IScoreObserver obs);
        void Notify(int newScore);
    }
}
