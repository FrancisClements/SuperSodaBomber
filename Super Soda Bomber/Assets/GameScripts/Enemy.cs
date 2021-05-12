using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/*
Enemy
    Responsible for handling the enemy properties
    and its behaviour.

    Things are needed to improve:
        Make sure that the script is flexible for all types of
        milkings. Except for Milcher. He'll have his own script

    Milkings:
        Shooter Milking
        Roller Milkling
*/

namespace SuperSodaBomber.Enemies{
    //base class template for milklings. all milkling classes will use this
    public abstract class BaseEnemy: MonoBehaviour, IEnemyOuter{
        //list of phase classes that are inside the BaseEnemy
        protected Dictionary<EnemyPhase, IEnemyInner> phaseDict = new Dictionary<EnemyPhase, IEnemyInner>();

        protected IEnemyInner chosenPhase;

        //enemies always start at wandering phase
        protected EnemyState currentState = EnemyState.Wander;
        //player position to track
        protected Vector3 playerPos;
        
        //data needed from scriptable object
        protected float spotRadius, attackRadius, attackRate;
        protected float attackRateMultiplier, healthMultiplier;
        protected bool facingRight;

        //animation
        protected Animator animator;

        //configures data
        public virtual void Init(Enemy_ScriptObject scriptObject, Transform attackSource, EnemyPhase phase){
            animator = gameObject.GetComponent<Animator>();
            spotRadius = scriptObject.spotRadius;
            attackRadius = scriptObject.attackRadius;
            facingRight = scriptObject.facingRight;
            attackRate = scriptObject.attackRate;

            if (phase == EnemyPhase.Phase2)
                attackRate /= scriptObject.attackRateMultiplier;
        }

        //flips the character if it's within the firing radius
        public void Flip(){
            //if the player:
                //is at the right and the enemy is facing left
                //is at the left and the enemy is facing right
            if(facingRight && (playerPos.x < transform.position.x) ||
            !facingRight && playerPos.x > transform.position.x){

                //flip it
                facingRight = !facingRight;
                CueFlipEvent();
                //Unlike the player, the enemy has a different shape of collider.
                transform.localScale = new Vector3(transform.localScale.x*-1, 1f, 1f);
            }
        }

        //returns true if the enemy is within the radius
        protected bool FindTarget(float radius){
            playerPos = PlayerMovement.playerPos;
            Debug.Log($"{Vector3.Distance(transform.position, playerPos)} vs {radius}");
            return (Vector3.Distance(transform.position, playerPos)  < radius);
        }

        //calls the state with the chosen phase
        public void InvokeState(){
            chosenPhase.CallState();
        }

        //must insert what happens when the enemy flips.
        protected virtual void CueFlipEvent(){}

        //must insert the firing/charge script at the enemy class.
        public abstract void CueAttack();

        //nested class for phases, since it will have different behavior
        public abstract class BaseInnerEnemy: IEnemyInner{
            //this is where the behavior takes place
            public abstract void CallState();
        }
    }

//ENEMY CLASSES

/*
    Shooter Milkling
        An enemy that shoots a milk projectile to the player.
*/

    public class Shooter: BaseEnemy{
        protected GameObject projectilePrefab;
        protected Transform attackSource;

        private bool shootToggle = false;

        public override void Init(Enemy_ScriptObject scriptObject, Transform attackSource, EnemyPhase phase)
        {
            base.Init(scriptObject, attackSource, phase);

            //add the phase sub-classes at the dictionary
            phaseDict.Add(EnemyPhase.Phase1, new Phase1(this));
            phaseDict.Add(EnemyPhase.Phase2, new Phase2(this));

            chosenPhase = phaseDict[phase];
            projectilePrefab = scriptObject.projectilePrefab;
            this.attackSource = attackSource;
        }

        protected void FireProjectile(){
            //get the angle by getting the target direction
            Vector3 targetDir = playerPos - attackSource.position;
            //use trigonometry to get the angle
            //then convert (y/x) rad to degrees
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            //convert to quaternion (datatype for rotations)
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            //spawn the projectile
            Instantiate(projectilePrefab, attackSource.position, q);
        }

        //used on burst
        protected void InvokeCoroutine(IEnumerator coro){
            StartCoroutine(coro);
        }
        
        public override void CueAttack(){
            shootToggle = !shootToggle;

            if(shootToggle)
                FireProjectile();
        }

        /// <summary>
        /// Shooter fires slowly
        /// </summary>
        public class Phase1: BaseInnerEnemy{
            //outer class to access the functions
            private Shooter outer;
            private float shootTime;

            public Phase1(Shooter o){
                outer = o;
                shootTime = Time.time;
            }

            public override void CallState(){
                switch (outer.currentState){
                    case EnemyState.Wander:
                        //find if the player is within the range
                        if (outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Attack;                
                        break;
                    case EnemyState.Attack:
                        //if it's within attacking range and it's within fire rate

                        Debug.Log($"C1: {outer.FindTarget(outer.attackRadius)}");
                        Debug.Log($"C2: {Time.time > shootTime}");
                        if (outer.FindTarget(outer.attackRadius) && Time.time > shootTime){
                            Debug.Log(outer.currentState);

                            //attack
                            outer.animator.SetTrigger("fire");
                            shootTime = Time.time + outer.attackRate;
                        }
                        //switch to wander if it's outside spot radius
                        else if (!outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Wander;

                        outer.Flip();
                        break;
                }
            }
        }

        /// <summary>
        /// Shooter fires on a burst
        /// </summary>
        public class Phase2: BaseInnerEnemy{
            private Shooter outer;
            private float shootTime;
            private float burstTime = 0.1f;
            private int shootAmount = 2;

            public Phase2(Shooter o){
                outer = o;
            }

            public override void CallState(){
                switch (outer.currentState){
                    case EnemyState.Wander:
                        //find if the player is within the range
                        if (outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Attack;                
                        break;
                    case EnemyState.Attack:
                        if (outer.FindTarget(outer.attackRadius) && Time.time > shootTime){
                            //attack
                            outer.InvokeCoroutine(Burst());
                            shootTime = Time.time + outer.attackRate;
                        }
                        else if (!outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Wander;

                        outer.Flip();
                        break;
                }
            }

            private IEnumerator Burst(){
                for (int i = 0; i < shootAmount; i++)
                {
                    outer.animator.SetTrigger("fire");                   
                    yield return new WaitForSeconds(burstTime);                    
                }
            }
        }
    }

    public class Roller: BaseEnemy{

        protected float rollSpeed;              //presetted rolling speed of the roller
        protected float speed = 0;              //current rolling speed of the roller
        protected Rigidbody2D rollRigid;        //rigidbody of the roller
        protected Vector3 rollVelocity = Vector3.zero; //used for referencing on movement
        private float rollSmoothing = .35f;     //"slippery" smotthing when rolling
        private float recoverSmoothing = .15f;  //less smoothing on idle
        protected bool attacking;               //roller is currently attacking
        protected bool allowSetCooldown;        //bool switch for cooldown

        private float rollTime = 3f;            //amount of time to roll
        private Coroutine rollCoroutine;        //asynchronous work

        public override void Init(Enemy_ScriptObject scriptObject, Transform attackSource, EnemyPhase phase)
        {
            base.Init(scriptObject, attackSource, phase);

            //add the phase sub-classes at the dictionary
            phaseDict.Add(EnemyPhase.Phase1, new Phase1(this));
            phaseDict.Add(EnemyPhase.Phase2, new Phase1(this));

            chosenPhase = phaseDict[phase];

            rollRigid = GetComponent<Rigidbody2D>();
            rollSpeed = scriptObject.movementSpeed * 25f;
        }

        private void Roll(float smoothing){
            //move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(speed * Time.fixedDeltaTime * 10f, rollRigid.velocity.y);

            //smoothen the velocity of the roller
            rollRigid.velocity = Vector3.SmoothDamp(rollRigid.velocity, targetVelocity, ref rollVelocity, smoothing);
        }

        protected override void CueFlipEvent(){
            //move to the opposite side when flipping
            speed *= -1;
        }

        private void StopRolling(){
            //set the animation to idle and reset the values
            animator.SetTrigger("idle");
            speed = 0;
            attacking = false;
            allowSetCooldown = true;
        }

        void FixedUpdate(){
            //use roll smoothing when attacking
            if (attacking)
                Roll(rollSmoothing);
            else if (speed == 0)
                Roll(recoverSmoothing);
        }

        public override void CueAttack(){
            attacking = true;
            rollCoroutine = StartCoroutine(SetRollingTime());
        }
        
        IEnumerator SetRollingTime(){
            yield return new WaitForSeconds(rollTime);
            StopRolling();
        }

        /// <summary>
        /// Roller Rolls
        /// </summary>
        public class Phase1: BaseInnerEnemy{
            //outer class to access the functions
            private Roller outer;
            private float attackTime;

            public Phase1(Roller o){
                outer = o;
                attackTime = Time.time;
            }

            public override void CallState(){
                switch (outer.currentState){
                    case EnemyState.Wander:
                        //find if the player is within the range
                        if (outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Attack;                
                        break;

                    case EnemyState.Attack:
                        //set the cooldown of the roller
                        if (outer.allowSetCooldown){
                            attackTime = Time.time + outer.attackRate;
                            outer.allowSetCooldown = false;
                        }
                        //if it's wihtin attacking range and it's within fire rate
                        else if (outer.FindTarget(outer.attackRadius) && Time.time > attackTime && !outer.attacking){
                            //attack
                            //sets speed (-speed if facing left)
                            outer.animator.SetTrigger("attack");
                            outer.speed = outer.rollSpeed * (outer.facingRight ? 1 : -1);
                            outer.allowSetCooldown = true;
                        }
                        //switch to wander if it's outside spot radius
                        else if (!outer.FindTarget(outer.spotRadius))
                            outer.currentState = EnemyState.Wander;

                        outer.Flip();
                        break;
                }
            }
        }

    }
}