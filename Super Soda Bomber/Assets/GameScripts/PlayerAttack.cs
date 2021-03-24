using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
PlayerAttack
    Used to trigger the attacking of the player
    and how the weapon fires

    Things are needed to improve:
        The script is hard-coded. It only provides attack
        to a fixed projectile with a fixed component/perk.

        This script/other scripts are needed to be flexible
        for projectiles/weapons with different perk and
        property.

        Components that are needed to be flexible with:
            Chosen Bomb/Weapon
            Perk
            Explosion (OK)

            Different behaviours caused by a perk (i.e. cluster bomb) (OK)
*/

public class PlayerAttack : PublicScripts
{
    //Attacking source of the player. This is where the projectile comes from
    public Transform attackSource;
    public Transform attackHandSource;

    //weapon prefab (fix this to make it more flexible)
    private GameObject projectilePrefab;
    public string projectileName;
    private bool isPrefabConfig;

    //firing properties
    private float fireRate;
    private float attackTime;

    private GameObject projectile;
    private ProjectileManager projectileScript;
    private bool isCreated;             //only applies to detonation projectiles. otherwise, it will stay false
    private explosionType explodeType;  //explosion type of the projectile. Located at PublicScripts.cs

    //asynchronous work
    private Coroutine coro;

    // Start is called before the first frame update
    void Awake()
    {
        ProjectileProcessor.Init();
        isCreated = false;
        isPrefabConfig = false;
        attackTime = 0;
    }

    public void Attack(bool isMoving, bool attack){
        //creates a projectile clone
		if (attack && (attackTime <= Time.time && !isCreated)){

            if (!isPrefabConfig){
                projectilePrefab = ProjectileProcessor.GetPrefab(projectileName);
                isPrefabConfig = true;
            }

            //set the shotgun location to attackhandsource
                if (projectileName == "shotgun")
                    projectile = Instantiate(projectilePrefab, attackHandSource.position, 
                    attackHandSource.rotation);
                else
                    projectile = Instantiate(projectilePrefab, attackSource.position, 
                    attackSource.rotation);

            //creates the projectile
            projectileScript = projectile.GetComponent<ProjectileManager>();

            //updates and fetches projectile's data
            projectileScript.SetPlayerMoving(isMoving);
            fireRate = fireRates[projectileScript.GetName()];
            explodeType = projectileScript.GetExplosionType();

            //adds the score and updates the attack time
            GameplayScript.current.AddScore(scores["fire"]);
            attackTime = fireRate + Time.time;

            //start waiting if it's a detonation projectile
            if (explodeType == explosionType.Detonate){
                Debug.Log("set isCreated to true");
                coro = projectileScript.coro;
                isCreated = true;
            }
		}

        //detonate the projectile using the button
        else if (attack && projectileScript != null && isCreated){
            Debug.Log("Detonated");

            StopCoroutine(coro);
            projectileScript.DetonateProjectile();
            isCreated = false;
        }

        //if the projectile exploded on its own
        else if (projectileScript == null && isCreated){
            isCreated = false;
        }
	}

}
