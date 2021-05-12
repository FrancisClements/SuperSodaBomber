using UnityEngine;
using SuperSodaBomber.Events;

/*
EnemyMovement
    Uses the Enemy AI and manages the health along with its event.
*/
namespace SuperSodaBomber.Enemies{
    public class EnemyMovement : MonoBehaviour, IDamageable
    {
        [SerializeField] private Enemy_ScriptObject scriptObject;   //holds saved data for the enemy
        [SerializeField] private VoidEvent enemyDeathEvent;         //contains the events when the enemy dies
        [SerializeField] private Transform attackSource;         //contains the events when the enemy dies
        [SerializeField] private EnemyPhase phase;
        private float health;

        private BaseEnemy chosenScript;
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            health = scriptObject.health;

            //gets the script using the enemy processor
            chosenScript = gameObject.AddComponent(EnemyProcessor.Fetch(scriptObject)) as BaseEnemy;
            chosenScript.Init(scriptObject, attackSource, phase);

            //change sprite and buff health if it's at phase 2
            if (phase == EnemyPhase.Phase2){
                health *= scriptObject.healthMultiplier;
            }
        }

        void FixedUpdate()
        {
            chosenScript.InvokeState();
            if(transform.position.y < 0){
                Die();
            }
        }

        public void Damage(float hp){
            health -= hp;

            if (health <=0){
                Die();
            }
        }

        void CueAttack(){
            chosenScript.CueAttack();
        }

        //when the enemy rans out of health
        void Die(){
            //calls the event and then disappears it
            enemyDeathEvent?.Raise();
            Destroy(gameObject);
        }
    }
}