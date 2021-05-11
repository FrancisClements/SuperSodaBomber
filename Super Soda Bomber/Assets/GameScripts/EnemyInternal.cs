/*
EnemyInternal
    Internal and essential data for enemy.
*/

namespace SuperSodaBomber.Enemies{
    /// 
    /// <summary>
    /// Interface for the Concrete Enemy Outer Class
    /// </summary>
    public interface IEnemyOuter{
        /// 
        /// <summary>
        /// Flips the sprite
        /// </summary>
        void Flip();
        /// <summary>
        /// Activates the attack using an animation event
        /// </summary>
        void CueAttack();
    }

    /// <summary>
    /// Interface for the Concrete Enemy Inner Class
    /// </summary>
    public interface IEnemyInner{
        /// 
        /// <summary>
        /// Calls the behavior of the enemy using a state.
        /// </summary>
        void CallState();
    }

    //Enemy Types
    public enum EnemyType{
        Shooter, Roller, Milcher
    }

    //Phases
    public enum EnemyPhase{
        Phase1, Phase2
    }

    //Enemy States
    public enum EnemyState{
        Wander, Chase, Attack
    }
}