﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperSodaBomber.Enemies;

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
    public string p_name = "SodaBomb";   //tag name
    protected Projectile_ScriptObject so;   //scriptable object
    protected List<string> targetLayers = new List<string>(); //enlists the target layers

    //data to save
    protected float throwX, throwY, spin;
    protected float detonateTime, blastRadius;

    //Initializes the class
    public virtual void Init(Projectile_ScriptObject scriptObject, 
    Rigidbody2D rigid, bool isMoving){

        so = scriptObject;

        spin = so.spin;
        throwX = so.throwX;
        throwY = so.throwY;
        detonateTime = so.detonateTime;
        blastRadius = so.blastRadius;

        ConfigVariables();

        //sets the throwing physics
        rigid.gravityScale = so.gravity ? 1 : 0;
        rigid.AddForce(new Vector2(0f, throwY));
        rigid.AddTorque(spin);

        //apply the moving player mechanic
        if (isMoving && so.applyMovingMechanic)
            throwX *= so.throwingMultiplier;

        rigid.velocity = transform.right * throwX;

    }

    protected virtual void ConfigVariables(){}

    protected virtual void Awake(){
        p_name = this.GetType().FullName;
    }

    public void GetDamageLayer(LayerMask layers){
        //if the layers have the "enemy" checked,
        if ((layers.value & 1 << LayerMask.NameToLayer("Enemy")) != 0){
            //add it to layers to damage
            targetLayers.Add("Enemy");
        }

        //same thing as the player
        if ((layers.value & 1 << LayerMask.NameToLayer("Player")) != 0){
            targetLayers.Add("Player");
        }
    }

    protected string VerifyLayer(LayerMask layer){
        //checks if it the layer is a target layer.
        foreach (string target in targetLayers)
        {
            //if it does, return the name
            if ((LayerMask.GetMask(target) & 1 << layer.value) != 0){
                return target;
            }
        }
        return null;
    }

    public void Explode(Collider2D col = null){
        //make target name null as default
        string targetName = null;

        //checks if the collider is a target
        if (col != null)
            targetName = VerifyLayer(col.gameObject.layer);

        Debug.Log("target name: " + targetName);
        //if it does not deal splash damage and hits the target
        if (col != null && targetName != null && !so.isSplashDamage){
            var targetScript = col.gameObject.GetComponent<IDamageable>();

            //if the target just died, don't damage it anymore
            if (targetScript == null)
                return;

            if (targetName == "Enemy"){
                //checks whether it has the key from PublicScripts.cs
                try{
                    GameplayScript.current.AddScore(projScores[p_name]);
                    targetScript.Damage(projDamage[p_name]);            
                }

                catch (KeyNotFoundException){
                    Debug.LogError($"Key '{p_name}' cannot be found at the PublicScripts.cs.");
                    targetScript.Damage(25);           
                }
            }
            else if (targetName == "Player"){
                targetScript.Damage();
            }
        }

        //if it deals splash damage
        else if (so.isSplashDamage){
            //gets a circlecast to get enemies that are within the blast radius
            var g_Collider = gameObject.GetComponent<BoxCollider2D>();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.transform.position, so.blastRadius);

            if(colliders.Length != 0){
                for(int i = 0; i< colliders.Length; ++i){
                    //if it hits a target
                    if(targetName != null){
                        //gets the distance between the target and the bomb
                        float distance = colliders[i].Distance(g_Collider).distance;
                        var targetScript = colliders[i].gameObject.GetComponent<IDamageable>();
                        
                        if (targetName == "Enemy")
                            targetScript.Damage(GetSplashDamage(Mathf.Abs(distance)));
                        else if (targetName == "Player")
                            targetScript.Damage();
                    }
                }   
            }
        }

        InvokeExplosionFX();
    }

    //this is the method overload of instant explosion
    public void Explode(GameObject prefab, int amount){
        for (int a = 0; a < amount; a++){
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }

    //instantiates explosion according to its amount
    protected void InvokeExplosionFX(){
        if (so.isExplosive){
            for (int amount = 0; amount < so.explosionAmount; amount++)
            {
                SpawnExplosion();
            }
        }
    }

    //instantiate code itself
    protected virtual void SpawnExplosion(){
        Instantiate(so.explosionPrefab, transform.position, Quaternion.identity);
    }

    protected float GetSplashDamage(float e_Distance){
        //inverts the value (closer distance, higher intensity)
        e_Distance = so.blastRadius - e_Distance;

        //gets the intensity (0% - 100%)
        float intensity = Mathf.RoundToInt((e_Distance/so.blastRadius)*100);
        
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

public class Fizztol: Projectile{}

/*
    Cannade
        A projectile that fires on a curve. When detonate 
        or waited within several seconds, it will let out
        a small group of cluster bombs
*/

public class Cannade: Projectile{

    //overrides the instantiate code and uses ForceRotation()
    protected override void SpawnExplosion(){
        Instantiate(so.explosionPrefab, gameObject.transform.position, ForceRotation());
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
    
    protected override void ConfigVariables(){
        //adds a random number generator for throwing physics
        throwX = 3f * UnityEngine.Random.Range(-.25f, 1.15f);
        throwY = UnityEngine.Random.Range(-100,100);
        detonateTime += UnityEngine.Random.Range(0f, .25f);
    }
}

/* 
    Shotgun (Sfizz)
        Fires short-ranged scattered pellets.
        Only used to spawn its pellets and then destroy itself
*/

public class Shotgun: Projectile{}

/*
    Shotgun Pellet (Sfizz internal)
        Small projectiles that inflict large damage the closer it hits the enemy
        It has a short reach.
*/

public class Pellet: Projectile{
    private float maxDistance = 5f;
    private Vector3 oldDistance;

    protected override void ConfigVariables(){
        //adds randomized x and y properties
        throwY = UnityEngine.Random.Range(-35f, 50f);
        throwX += UnityEngine.Random.Range(-.75f, 1.2f);
        oldDistance = gameObject.transform.position;
    }

    void Update(){
        //updates the distance. if it exceeds the max distance, despawn
        if (GetDistance(gameObject.transform.position) >= maxDistance)
            Destroy(gameObject);

    }

    float GetDistance(Vector3 newDistance){
        Vector3 gap = oldDistance - newDistance;
        return gap.sqrMagnitude;
    }
    
}

//ENEMY PROJECTILE TYPES
public class ShooterProjectile: Projectile{}