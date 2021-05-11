using System.Collections;
using SuperSodaBomber.Events;
using UnityEngine;


namespace SuperSodaBomber.Enemies{    
    public class Milcher : MonoBehaviour, IDamageable
    {

        //Enemy State
        public enum MilcherState{
            Charge,
            Attack,
            Fallback,
            Idle
        }


        [SerializeField] private VoidEvent onMilcherDeath;  //events when milcher dies and rans out of health
        [SerializeField] private Animator animator;

        [Space]
        [Header("Milcher Death")]
        [SerializeField] private GameObject finishFlag;
        [SerializeField] private Transform flagSource;

        [Space]
        [Header("Character Info")]
        [SerializeField] [Range(200f, 2000f)] protected float health;            //health of milcher
        [Range(.5f, 60f)] public float cooldown;          //cooldown after firing
        [Range(6f, 25f)] public float tooFarRadius;      //radius if player is too far
        [Range(0, 5f)] public float tooCloseRadius;    //radius if player is too close
        [Range(1f, 50f)] public float movementSpeed;     //movement speed of milcher

        [Space]
        [Header("Sprites")]
        [SerializeField] protected SpriteRenderer[] parts;    //sprite parts to color (for damage and death)
        [SerializeField] private Transform[] fireSources;

        //internal stuff
        [HideInInspector] public int phase = 1;
        [HideInInspector] public MilcherState currentState;
        private Coroutine colorCoro;
        private bool isDead;
        private float maxHealth;

        //color overlay for damage animation
        private Color damageColor = new Color(1f, .78f, .78f, 1f);
        private Color fadeColor = new Color(1f, 1f, 1f, 0);

        // Start is called before the first frame update
        protected virtual void Start()
        {
            movementSpeed *= 2f;
            maxHealth = health;
        }

        public IEnumerator SpawnProjectile(GameObject prefab, int amount, float delay, Quaternion rotation){
            Transform selectedSource;

            if (fireSources.Length == 1)
                selectedSource = fireSources[0];
            else
                selectedSource = fireSources[Random.Range(0, fireSources.Length)];

            if(fireSources.Length != 0){
                for (int i = 0; i < amount; i++)
                {   
                    Instantiate(prefab, selectedSource.position, rotation);

                    yield return new WaitForSeconds(delay);
                }
            }

        }

        public void Damage(float hp){
            health -= hp;

            Debug.Log(health);

            //activate phase 2 if it reaches half health
            if (health < (maxHealth/2) && phase == 1){
                phase++;
                UpdatePhaseStat();
            }
            else if (health > 0){
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

        //hardcoded
        private void UpdatePhaseStat(){
            movementSpeed *= 1.5f;
        }

        private IEnumerator Die(){
            //waits for the fade before destroying the object
            gameObject.layer = LayerMask.NameToLayer("Phantom");
            currentState = MilcherState.Idle;
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

        public void PlayAnim(string animation){
            animator.Play(animation);
        }

        void Update(){
            if (transform.position.y <= 0)
                Die();
        }
    }
}