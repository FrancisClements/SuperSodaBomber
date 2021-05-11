using UnityEngine;
using System.Collections;

/*
Projectile Manager
    Responsible for handling the projectile properties
    and its behavior such as handling how the projectile
    explodes
*/

public class ProjectileManager : PublicScripts
{
    [SerializeField] private Projectile_ScriptObject scriptObject;    //projectile scriptable object

    //projectile attributes
    private bool playerMoving;          //is the player fires while moving
    public Rigidbody2D rigid;           //rigidbody2D of the projectile prefab
    private Projectile s_Projectile;    //projectile script of the prefab

    //determines on which layers destroys the projectile
    [SerializeField] private LayerMask layersToCollide;

    //used on detonations
    public Coroutine coro;

    //forces projectile to destroy itself with the time limit
    private float spawnDuration = 5f;

    void Awake()
    {
        //adds component
        s_Projectile = ProjectileProcessor.ConfigureComponent(gameObject);
        //activates delayed detonation for some projectiles
        ExplosionType explodeType = GetExplosionType();

        /*
            CASE 1:
                - if it's animated projectile
                - AND the sprite list size isn't 0
                - AND the script inherits IAnimatedProjectile interface

            CASE 2:
                - if projectile is either a delay or detonate-type

            CASE 3:
                - if projectile is an instant-type
        */ 

        if (explodeType == ExplosionType.Delay ||
            explodeType == ExplosionType.Detonate)
                coro = StartCoroutine(WaitUntilDetonate());

        //instantly explode this one because why not
        else if (explodeType == ExplosionType.Instant)
            ExplodeProjectile(scriptObject.explosionPrefab, scriptObject.explosionAmount);
        
        else
            coro = StartCoroutine(SetDespawnTime());
    }

    void Start(){
        //central throwing attributes
        s_Projectile.Init(scriptObject, rigid, playerMoving);
        s_Projectile.GetDamageLayer(layersToCollide);

        if (typeof(IAnimatedProjectile).IsAssignableFrom(s_Projectile.GetType())){

                //convert script into animated projectile class
                var s_animProj = (IAnimatedProjectile) s_Projectile;
                
                //call the custom detonation
                if (coro != null)
                    StopCoroutine(coro);

                coro = StartCoroutine(s_animProj.WaitUntilDetonate());
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        //detects whether if the projectile collides with the map or the enemy
        if ((layersToCollide.value & 1 << col.gameObject.layer) != 0 && 
            (GetExplosionType() != ExplosionType.Delay) && 
            col.gameObject.tag != "WallDetector"){
            //if it collides, activate the particle effect and then destroy the Bomb projectile.
            ExplodeProjectile(col);
        }
    }

    /// <summary>
    /// Waits for an amount of time and then automatically detonates it.
    /// </summary>
    public IEnumerator WaitUntilDetonate(){
        //sets up waiting time
        float waitingTime = GetDetonateTime();

        //waits for a short amount of time before exploding/despawning it.
        yield return new WaitForSeconds(waitingTime);
        DetonateProjectile();
    }

    /// <summary>
    /// Sets the timer for despawning
    /// </summary>
    private IEnumerator SetDespawnTime(){
        yield return new WaitForSeconds(spawnDuration);
        DetonateProjectile();
    }

    /// <summary>
    /// Explodes the projectile without the use of colliders
    /// </summary>
    public void DetonateProjectile(){
        ExplodeProjectile();
    }
    
    /// <summary>
    /// Explodes the projectile.
    /// </summary>
    /// <param name="col">Collider Info</param>
    public void ExplodeProjectile(Collider2D col = null){
        s_Projectile.Explode(col);
        Destroy(gameObject);
    }

    /// <summary>
    /// Explodes the projectile instantly.
    /// </summary>
    /// <param name="prefab">Explosion Prefab</param>
    /// <param name="amount">Amount of Explosion</param>
    public void ExplodeProjectile(GameObject prefab, int amount){
        s_Projectile.Explode(prefab, amount);
        Destroy(gameObject);
    }
    //getters and setters
    /// <summary>
    /// Updates the PlayerMoving.
    /// </summary>
    /// <param name="moving">status of the player movement</param>
    public void SetPlayerMoving(bool moving){
        playerMoving = moving;
    }

    /// <returns>Explosion Type of the projectile</returns>
    public ExplosionType GetExplosionType(){
        return scriptObject.explosionType;
    }

    /// <returns>Projectile Name</returns>
    public string GetName(){
        return s_Projectile.p_name;
    }

    /// <returns>Detonation Time of the Projectile</returns>
    public float GetDetonateTime(){
        return scriptObject.detonateTime;
    }
}