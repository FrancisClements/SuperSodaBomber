using UnityEngine;
using System.Collections;

/*
Projectile
    Responsible for handling the projectile properties
    and its behavior such as handling how the projectile
    explodes
*/

public class ProjectileManager : PublicScripts
{
    //selects what kind of projectile is it to change the properties
    [HideInInspector]
    public enum Type{
        Bomb, Pistol, Cluster, smallCluster, Shotgun, pellet
    }

    //the selected property
    public Type type;

    //projectile attributes
    private bool playerMoving;          //is the player fires while moving
    public Rigidbody2D rigid;           //rigidbody2D of the projectile prefab
    private Projectile s_Projectile;    //projectile script of the prefab

    //determines on what destroys the projectile
    [SerializeField] private LayerMask layersToCollide;

    //particle system (explosion)
    public GameObject explosion;

    //asynchronous work
    public Coroutine coro;

    void Awake()
    {
        //adds component
        s_Projectile = ProjectileProcessor.ConfigureComponent(gameObject);
        //activates delayed detonation for some projectiles
        explosionType explodeType = GetExplosionType();

        if (explodeType == explosionType.Delay ||
            explodeType == explosionType.Detonate)
                coro = StartCoroutine(WaitUntilDetonate());

        //instantly explode this one because why not
        else if (explodeType == explosionType.Instant)
            ExplodeProjectile();
    }

    void Start(){
        //central throwing attributes
        s_Projectile.Init(rigid, playerMoving);
    }

    void OnTriggerEnter2D(Collider2D col){
        //detects whether if the projectile collides with the map or the enemy
        if ((layersToCollide.value & 1 << col.gameObject.layer) != 0 && 
            (GetExplosionType() != explosionType.Delay)){
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
    /// Explodes the projectile without the use of colliders
    /// </summary>
    public void DetonateProjectile(){
        ExplodeProjectile();
    }
    
    /// <summary>
    /// Explodes the projectile.
    /// </summary>
    /// <param name="col">collider message</param>
    public void ExplodeProjectile(Collider2D col = null){
        s_Projectile.Explode(col, explosion);
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
    public Projectile.explosionType GetExplosionType(){
        return s_Projectile.selectedType;
    }

    /// <returns>Projectile Name</returns>
    public string GetName(){
        return s_Projectile.p_name;
    }

    /// <returns>Detonation Time of the Projectile</returns>
    public float GetDetonateTime(){
        return s_Projectile.detonateTime;
    }
}