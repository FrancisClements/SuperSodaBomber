using System.Collections;
using SuperSodaBomber.Events;
using UnityEngine;


namespace SuperSodaBomber.Enemies{    
    public class Milcher : MonoBehaviour, IDamageable
    {
        //Milcher Modes
        private enum MilcherPhase{
            Tank, Mech
        }

        //Enemy State
        protected enum MilcherState{
            Charge,
            Attack,
            Fallback,
            Idle
        }

        [SerializeField] private MilcherPhase phase;
        [SerializeField] private VoidEvent onMilcherDeath;  //events when milcher dies and rans out of health
        [SerializeField] private Animator animator;

        [Space]
        [Header("Milcher Death")]
        [SerializeField] private GameObject finishFlag;
        [SerializeField] private Transform flagSource;

        [Space]
        [Header("Character Info")]
        [SerializeField] [Range(200f, 2000f)] protected float health;            //health of milcher
        [SerializeField] [Range(.5f, 60f)] protected float cooldown;          //cooldown after firing
        [SerializeField] [Range(6f, 25f)] protected float tooFarRadius;      //radius if player is too far
        [SerializeField] [Range(0, 5f)] protected float tooCloseRadius;    //radius if player is too close
        [SerializeField] [Range(1f, 50f)] protected float movementSpeed;     //movement speed of milcher

        [Space]
        [Header("Sprites")]
        [SerializeField] protected SpriteRenderer[] parts;    //sprite parts to color (for damage and death)

        //internal stuff
        protected MilcherState currentState;
        private Coroutine colorCoro;
        private IEnemyOuter script;
        private bool isDead;

        //color overlay for damage animation
        private Color damageColor = new Color(1f, .78f, .78f, 1f);
        private Color fadeColor = new Color(1f, 1f, 1f, 0);

        // Start is called before the first frame update
        protected virtual void Start()
        {
            animator = GetComponent<Animator>();

            switch (phase){
                case MilcherPhase.Tank:
                    script = gameObject.AddComponent<MilcherTank>();
                    break;

                case MilcherPhase.Mech:
                    //innerScript =  gameObject.AddComponent<MilcherMech>();
                    break;
            }
        }

        public void Damage(float hp){
            health -= hp;

            if (health > 0){
                Debug.Log(health);

                if (colorCoro != null)
                    StopCoroutine(colorCoro);

                colorCoro = StartCoroutine(ColorParts(damageColor, Color.white));
            }
            else if (!isDead){
                StartCoroutine(Die());
                isDead = true;
            }
        }

        private IEnumerator Die(){
            //waits for the fade before destroying the object
            gameObject.layer = LayerMask.NameToLayer("Phantom");
            yield return StartCoroutine(ColorParts(Color.white, fadeColor, 3));
            Instantiate(finishFlag, flagSource.position, Quaternion.identity);
            onMilcherDeath?.Raise();
            Destroy(gameObject);
        }

        protected IEnumerator ColorParts(Color fromColor, Color targetColor, float secs = .15f){
            int slices = 25;
            float rate = secs/slices;
            Color lerpColor;

            for (int i = 0; i < slices; ++i){
                lerpColor = Color.Lerp(fromColor, targetColor, (float) (i+1)/slices);
                for (int j = 0; j < parts.Length; ++j){
                    parts[j].color = lerpColor;
                }
                //ex: .15f second per 25 slices
                yield return new WaitForSeconds(rate);
            }

            colorCoro = null;
        }

        void Update(){
            if (transform.position.y <= 0)
                Die();
            
        }
    }
}