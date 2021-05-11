using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSodaBomber.Events;

//MILCHER TANK
namespace SuperSodaBomber.Enemies{
    public class MilcherTank: BaseEnemy{

        [SerializeField] private Milcher mainScript;
        [SerializeField] private GameObject turret;
        [SerializeField] private Transform attackSource;

        [Space]
        [Header("Projectiles")]
        [SerializeField] private GameObject machineGun;
        [SerializeField] private GameObject tankShell;

        private Rigidbody2D rigid;
        private Vector3 refVelocity = Vector3.zero;
        private float moveSmoothing = .25f;
        private float speed = 0;
        private bool hasFlipped = false;

        private Coroutine attackCoro;

        void Start(){
            rigid = GetComponent<Rigidbody2D>();
            facingRight = false;
            speed = mainScript.movementSpeed * -1;
        }

        public new void Flip(){
            facingRight = !facingRight;
            transform.localScale = new Vector3(transform.localScale.x*-1, 1f, 1f);
            speed *= -1;
            hasFlipped = true;
        }

        public void FlipToPlayer(){
            if(facingRight && (playerPos.x < transform.position.x) ||
            !facingRight && playerPos.x > transform.position.x){
                Flip();
            }
        }

        //CueAttack only works if it attacks by syncing it with animation
        public override void CueAttack(){}

        public void SetStateToAttack(){
            mainScript.currentState = Milcher.MilcherState.Attack;
        }

        private void Move(){
            mainScript.PlayAnim("MoveForward");

            Vector3 targetVelocity = new Vector2(speed * Time.fixedDeltaTime * 10f, rigid.velocity.y);

            rigid.velocity = Vector3.SmoothDamp(rigid.velocity, targetVelocity, ref refVelocity, moveSmoothing);
        }

        public void TrackPlayer(){
            playerPos = PlayerMovement.playerPos;
        }

        private IEnumerator AimTurret(Quaternion targetAngle){
            int slices = 25;

            for (int i = 0; i < slices; i++)
            {
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetAngle, 1f/slices);
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator StartAttack(){
            //get the angle between the turret and the player
            Vector3 targetDir = playerPos - attackSource.position;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            //add 180 degrees as offset if it's facing left (turret sprite does not cooperate)
            if (!facingRight)
                angle += 180;

            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

            //now that the angles is ready, slowly point it to the player
            yield return StartCoroutine(AimTurret(q));

            //subtract 180, for attacking
            if (!facingRight){
                angle -= 180;
                q = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            //attack
            if (mainScript.phase == 1)
                yield return StartCoroutine(mainScript.SpawnProjectile(machineGun, 10, .05f, q));
            else{
                StartCoroutine(mainScript.SpawnProjectile(machineGun, 10, .05f, q));
                yield return StartCoroutine(mainScript.SpawnProjectile(tankShell, 2, 1f, q));
            }

            //reset angle
            var identity = Quaternion.Euler(0,0,0);
            yield return StartCoroutine(AimTurret(identity));

            //if it hasn't flipped yet, flip it
            if (!hasFlipped){
                Flip();
            }

            //add cooldown if it's phase 2
            if (mainScript.phase == 2){
                yield return new WaitForSeconds(mainScript.cooldown);
            }

            hasFlipped = false;
            attackCoro = null;

            if (mainScript.currentState != Milcher.MilcherState.Idle)
                mainScript.currentState = Milcher.MilcherState.Charge;
        }

        public void Attack(){
            if (attackCoro == null){
                TrackPlayer();
                FlipToPlayer();
                mainScript.PlayAnim("Idle");
                attackCoro = StartCoroutine(StartAttack());
            }
        }
        
        void FixedUpdate(){
            if (mainScript.currentState == Milcher.MilcherState.Idle){
                if(attackCoro != null)
                    StopCoroutine(attackCoro);
            }
            else
                CallState();
        }

        private void CallState(){
            switch(mainScript.currentState){
                case Milcher.MilcherState.Charge:
                    Move();
                    break;
                case Milcher.MilcherState.Attack:
                    Attack();
                    break;
            }
        }   
    }
}

