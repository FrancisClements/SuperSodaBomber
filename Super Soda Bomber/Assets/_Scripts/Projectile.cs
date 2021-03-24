using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
    Projectile
        Contains each of the projectile's attributes
        and behaviors

        Fizzy
            Soda Bomb
            Fizztol (Pistol)
            Cannade (Cluster Bomb)
            Sfizz (Shotgun)

        Enemy
            Milk Shooter
            Milcher's Rifle

*/

//main class. all projectiles will inhertit this class
public abstract class Projectile: PublicScripts{

    /*it turns out that using "protected" keyword can be used
    by the class itself and by the class that inherits it.
    
    while "private" keyword can be used by the class itself.*/


    //metadata
    /// <summary>Projectile name</summary>
    public string p_name = "sodaBomb";

    //projectile attributes
    //throwing physics
    protected float throwX = 3f,
                    throwY = 250f,
                    spin = 200f,
                    gravity = 1f;
    
    //explosion attributes
    protected float blastRadius = 2.5f;
    /// <summary>Provides blast damage + explosion fx </summary>
    protected bool isExplosive = true;
    /// <summary>Selected Explosion Type</summary>
    public explosionType selectedType;
    /// <summary>Time until the projectile explodes by itself</summary>
    public float detonateTime = 0f;


    //moving player mechanic 
    /// <summary>Adds a multiplier to throwX when the player is moving</summary>
    protected float throwingMultiplier = 2.5f;
    protected bool applyMovingMechanic = true;

    public virtual void Init(Rigidbody2D rigid, bool isMoving){
        rigid.gravityScale = gravity;
        rigid.AddForce(new Vector2(0f, throwY));
        rigid.AddTorque(spin);

        //apply the moving player mechanic
        if (isMoving && applyMovingMechanic)
            throwX *= throwingMultiplier;

        rigid.velocity = transform.right * throwX;
    }

    //virtual enables overriding of functions on inherited classes

    public virtual void Explode(Collider2D col = null, GameObject explosion = null){
        //if it's not an explosive and directly hits the enemy and collider is not empty
        if (col != null && col.gameObject.tag == "Enemy" && !isExplosive){
            var enemyScript = col.gameObject.GetComponent<Enemy>();

            //checks whether it has the key from PublicScripts.cs
            try{
                GameplayScript.current.AddScore(projScores[p_name]);
                enemyScript.Damage(projDamage[p_name]);            
            }

            catch (KeyNotFoundException){
                Debug.LogError($"Key '{p_name}' cannot be found at the PublicScripts.cs.");
                enemyScript.Damage(25);           
            }
        }
        else{
            //gets a circlecast to get enemies that are within the blast radius
            var g_Collider = gameObject.GetComponent<BoxCollider2D>();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, blastRadius);

            if(colliders.Length != 0){
                for(int i = 0; i< colliders.Length; ++i){
                    if(colliders[i].gameObject.tag == "Enemy"){
                        //gets the distance between the enemy and the bomb
                        float distance = colliders[i].Distance(g_Collider).distance;
                        var enemyScript = colliders[i].gameObject.GetComponent<Enemy>();
                        //damage the enemy
                        enemyScript.Damage(GetSplashDamage(Mathf.Abs(distance)));
                    }
                }   
            }
        }

        if(explosion != null)
            Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
    }

    protected float GetSplashDamage(float e_Distance){
        //inverts the value (closer distance, higher intensity)
        e_Distance = blastRadius - e_Distance;

        //gets the intensity (0% - 100%)
        float intensity = Mathf.RoundToInt((e_Distance/blastRadius)*100);
        
        try
        {
            if (intensity < 20) {
                GameplayScript.current.AddScore(projScores[$"{p_name}_s"]);
                return projDamage[$"{p_name}_min"];
            }
            else if (intensity < 75) {
                GameplayScript.current.AddScore(projScores[$"{p_name}_m"]);
                return Mathf.RoundToInt(projDamage[$"{p_name}_max"]/2);
            }

            //if distance is <= 75% intensity (almost a direct hit)
            GameplayScript.current.AddScore(projScores[$"{p_name}_l"]);
            return projDamage[$"{p_name}_max"];
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Key '{p_name}' is missing at the PublicScripts.cs.");
            return 25f;           
        }
    }
}

//PROJECTILE TYPES

/*
    Soda Bomb
        A projectile that fires on a curve. It explodes on contact.
        This is Fizzy's stock weapon.
*/

//Default values of Projectile is SodaBomb
public class SodaBomb: Projectile{}

/*
    Fizztol
        A projectile that fires on a straight line.
        It attacks enemy on contact and doesn't explode.
*/

public class Fizztol: Projectile{

    void Awake(){
        p_name = "fizztol";
        //throwing physics
        throwX = 4f;
        throwY = 0;
        gravity = 0;

        //explosion & player moving mechanic
        isExplosive = false;
        applyMovingMechanic = false;
    }
}

/*
    Cannad
        A projectile that fires on a curve. When detonate 
        or waited within several seconds, it will let out
        a small group of cluster bombs
*/

public class Cannade: Projectile{

    private int clusterAmount = 5;

    void Awake(){
        selectedType = explosionType.Detonate;
        spin = 10f;
        p_name = "cannade";
        detonateTime = 2f;

    }

    public override void Explode(Collider2D col, GameObject explosion){
        //spawns small cluster bomb
        for (int i = 0; i < clusterAmount; ++i){
            Instantiate(explosion, gameObject.transform.position, ForceRotation());
        }
    }

    //forces the z rotation to 0 or 180
    private Quaternion ForceRotation(){
        float yRotation = gameObject.transform.rotation.eulerAngles[1];
        Quaternion newRotation = Quaternion.Euler(0f, yRotation, 0f);

        return newRotation;
    }
}

/*
    Small Cluster (Cannade Phase 2)
        Small bomb that spawns in a small group from the Cannade Phase 1.
        It provides a small blast radius, damage and explodes in set time
*/

public class SmallCluster: Projectile{

    
    void Awake(){
        p_name = "smallCluster";
        selectedType = explosionType.Delay;
        throwX *= UnityEngine.Random.Range(-.25f, 1.15f);
        throwY = UnityEngine.Random.Range(-100,100);
        blastRadius = 1.5f;
        applyMovingMechanic = false;
        detonateTime = 3f + UnityEngine.Random.Range(0f, .25f);
    }
}

/* 
    Shotgun (Sfizz)
        Fires short-ranged scattered pellets.
        Only used to spawn its pellets and then destroy itself
*/

public class Shotgun: Projectile{

    private int pellets = 8;
    private Vector3 attackSource;

    void Awake(){
        p_name = "shotgun";
        selectedType = explosionType.Instant;
        //lowers the y-value for attack source of the pellets
        // attackSource = gameObject.transform.position + new Vector3(.2f, -.45f, 0f);
    }

    public override void Explode(Collider2D col, GameObject explosion){
        for(int i = 0; i < pellets; ++i){
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}

/*
    Shotgun Pellet (Sfizz) (internal)
        Small projectiles that inflict large damage the closer it hits the enemy
        It has a short reach.
*/

public class Pellet: Projectile{
    private float distance = 0f;
    private float maxDistance = 5f;
    private Vector3 oldDistance;
    private Vector3 newDistance;


    void Awake(){
        p_name = "shotgun";
        //adds randomized x and y properties
        throwY = UnityEngine.Random.Range(-35f, 50f);
        throwX += 1.5f + UnityEngine.Random.Range(-.75f, 1.2f);
        gravity = 0;
        applyMovingMechanic = false;
        oldDistance = gameObject.transform.position;
    }


    public override void Explode(Collider2D col = null, GameObject explosion = null)
    {
        if (col != null && col.gameObject.tag == "Enemy"){
            newDistance = gameObject.transform.position;
            var enemyScript = col.gameObject.GetComponent<Enemy>();

            //gets the distance, damage it and adds the score
            try{
                GameplayScript.current.AddScore(projScores[p_name]);
                enemyScript.Damage(projDamage[p_name]);            
            }

            catch (KeyNotFoundException){
                Debug.LogError($"Key '{p_name}' cannot be found at the PublicScripts.cs.");
                enemyScript.Damage(25);           
            }
        }

        //explosion fx
        if (explosion != null){
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        }
    }

    void Update(){
        //updates the distance. if it exceeds the max distance, despawn
        distance = GetDistance(gameObject.transform.position);
        if (distance >= maxDistance)
            Destroy(gameObject);
        newDistance = gameObject.transform.position;

    }

    float GetDistance(Vector3 newDistance){
        Vector3 gap = oldDistance - newDistance;
        return gap.sqrMagnitude;
    }
    
}
