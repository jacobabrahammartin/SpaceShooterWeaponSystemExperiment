using UnityEngine;


namespace SpaceShooterFinal
{
    public interface IFiringStrategy
    {
        void Initialize(DarkShot darkShot);
        void Execute();
    }

    public class NormalFiringStrategy : IFiringStrategy
    {
        private DarkShot darkShot;

        public void Initialize(DarkShot darkShot)
        {
            this.darkShot = darkShot;
        }

        public void Execute()
        {
            darkShot.FireDarkShot(darkShot.GetRandomFirePoint());
        }
    }

    public class TripleShotFiringStrategy : IFiringStrategy
    {
        private DarkShot darkShot;

        public void Initialize(DarkShot darkShot)
        {
            this.darkShot = darkShot;
        }

        public void Execute()
        {
            darkShot.FireTripleDarkShot(darkShot.GetRandomFirePoint());
        }
    }

    public class InfiniteTripleShotFiringStrategy : IFiringStrategy
    {
        private DarkShot darkShot;

        public void Initialize(DarkShot darkShot)
        {
            this.darkShot = darkShot;
        }

        public void Execute()
        {
            darkShot.FireInfiniteTripleDarkShot(darkShot.GetRandomFirePoint());
        }
    }
}