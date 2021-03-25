using System;

namespace AI_AStarCraft.Simulations
{
    public abstract class GameObject
    {
        public int Id { get; }
        private static int number;
        
        protected GameObject()
        {
            Id = number++;
        }

        public abstract void Interact(GameObject other);

        public override bool Equals(object? obj)
            => obj is GameObject gameObject && Equals(gameObject);

        protected bool Equals(GameObject other)
            => Id.Equals(other.Id);

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}