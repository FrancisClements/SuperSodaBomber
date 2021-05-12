using UnityEngine;

/*
PlayerAttack
    Used to trigger the attacking of the player
    and how the weapon fires
*/

public class PlayerAttack : PublicScripts
{
    //Attacking source of the player. This is where the projectile comes from
    public Transform attackSource;
    public Transform attackHandSource;

    //projectile properties
    private GameObject projectilePrefab;        //projectile gameobject
    public PlayerProjectiles chosenProjectile;  //chosen projectile (enum)
    private string projectileName;              //name of the projectile
    private bool isPrefabLoaded;                //state if the prefab is loaded

    //firing properties
    private float fireRate;                             //lower = faster
    public float rateMultiplier { get; private set; }   //modifies firerate non-destructively
    private float attackTime;                           //time when attack can be activated again

    private GameObject projectile;              //cloned projectile prefab
    private ProjectileManager projectileScript; //projectile script
    private bool isCreated;                     //only applies to detonation projectiles. otherwise, it will stay false
    private ExplosionType explodeType;          //explosion type of the projectile. Located at PublicScripts.cs

    //asynchronous work (used on detonation-type projectiles)
    private Coroutine coro;

    // Start is called before the first frame update
    void Awake()
    {
        ProjectileProcessor.Init();
        isCreated = false;
        isPrefabLoaded = false;
        attackTime = 0;
        rateMultiplier = 1f;
    }

    /// <summary>
    /// Updates the Attack Rate Multiplier.
    /// </summary>
    /// <param name="newValue">New Multiplier Value</param>
    public void SetAttackRateMultiplier(float newValue){
        rateMultiplier = newValue;
    }

    /// <summary>
    /// Makes the player attack.
    /// </summary>
    /// <param name="isMoving">Player's movement state</param>
    /// <param name="attack">Player's attacking state</param>
    public void Attack(bool isMoving, bool attack){

        //if the following conditions apply:
            //didnt attack,
            //currently on firerate cooldown,
            //projectile has spawned already (detonation-type only)
        //do nothing

		if (attack && (attackTime <= Time.time && !isCreated)){
            //use the attack animation
            switch(chosenProjectile){
                case PlayerProjectiles.Fizztol:
                    PlayerAnimation.current.ChangeAnimState("FIRE_FIZZTOL");
                    break;
                case PlayerProjectiles.Shotgun:
                    PlayerAnimation.current.ChangeAnimState("FIRE_SHOTGUN");
                    break;
                default:
                    PlayerAnimation.current.ChangeAnimState("THROW");
                    break;
            }

            //load the projectile
            if (!isPrefabLoaded){
                projectilePrefab = ProjectileProcessor.GetPrefab(chosenProjectile);
                projectileName = ProjectileProcessor.GetProjectileName();
                isPrefabLoaded = true;
            }

            //set the shotgun location to attackhandsource and spawn one
                if (projectileName == "Shotgun" || projectileName == "Fizztol")
                    projectile = Instantiate(projectilePrefab, attackHandSource.position, 
                    attackHandSource.rotation);
                else
                    projectile = Instantiate(projectilePrefab, attackSource.position, 
                    attackSource.rotation);

            //creates the projectile
            projectileScript = projectile.GetComponent<ProjectileManager>();

            //updates and fetches projectile data
            projectileScript.SetPlayerMoving(isMoving);
            fireRate = fireRates[projectileScript.GetName()]/rateMultiplier;
            explodeType = projectileScript.GetExplosionType();

            //updates the attack time
            attackTime = fireRate + Time.time;

            //start waiting if it's a detonation projectile
            if (explodeType == ExplosionType.Detonate){
                coro = projectileScript.coro;
                isCreated = true;
            }
		}

        //detonate the projectile using the button
        else if (attack && projectileScript != null && isCreated){
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
