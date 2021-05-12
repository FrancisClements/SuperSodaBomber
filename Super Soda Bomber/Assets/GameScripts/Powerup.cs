using System.Collections;
using UnityEngine;


/*
Powerup
    Manages and uses the powerups when needed.
        - SugarRush
        - 1UP
*/

/// <summary>
/// Abilities that enhances one of the player's stats at game proper.
/// </summary>
public class Powerup: MonoBehaviour{
    public enum PowerupType{SugarRush, OneUP}       //powerup types
    public PowerupType key;                         //selected powerup

    private IPowerup powerupComponent;              //powerup script

    void OnTriggerEnter2D(Collider2D col){
        //if it's not a player, do nothing
        if(col.gameObject.layer != 8 && col.gameObject.layer != 13)
            return;

        //gets the PlayerControl script
        var playerControl = col.gameObject.GetComponent<PlayerControl>();

        //adds the selected powerup to the player
        switch (key){
            case PowerupType.SugarRush:
                var obj = new SugarRush();
                playerControl.AddPowerup(obj);

                //calls the UI
                UICooldownDebug.current.CallCooldownUI("SugarRush",
                    "score", obj.abilityDuration);
                break;
            case PowerupType.OneUP:
                playerControl.AddPowerup(new OneUP());
                break;
        }
        
        Destroy(gameObject);
    }
}

/*
    SugarRush
        Adds temporary x2 Attack Speed to the Player
*/

public class SugarRush: IPowerup, IDurationPowerup
{
    public float abilityDuration { get; private set; } = 10; //how long the effect last
    private float multiplier = 1.25f;           //attack speed multiplier
    private float oldMultiplier;            //old attack speed multiplier
    private PlayerAttack playerAtk;         //player attack script
    private PlayerControl playerControl;    //player control script

    public void Apply(GameObject player){
        //gets the player scripts
        playerAtk = player.GetComponent<PlayerAttack>();
        playerControl = player.GetComponent<PlayerControl>();

        //stores the current value
        oldMultiplier = playerAtk.rateMultiplier;

        //updates the speed multiplier
        playerAtk.SetAttackRateMultiplier(multiplier);
        Debug.Log("sugar rush has started!");
    }

    /// <summary>Waits for duration and then unapply the ability.</summary>
    public IEnumerator AbilityEffect()
    {
        yield return new WaitForSeconds(abilityDuration);
        Debug.Log("sugar rush has ended!");

        //revert to old value and then remove the powerup
        playerAtk.SetAttackRateMultiplier(oldMultiplier);
        playerControl.RemovePowerup(this);
    }
}

/*
    1UP
        Adds extra health to the player.
*/

public class OneUP: IPowerup{
    public void Apply(GameObject player){
        var playerControl = player.GetComponent<PlayerControl>();
        var playerHealth = player.GetComponent<PlayerHealth>();

        //call the function and then remove the powerup
        playerHealth.AddHP();
        playerControl.RemovePowerup(this);
    }
}

/// <summary>
/// /// Interface for Powerups
/// </summary>
public interface IPowerup{

    /// <summary>
    /// Configures and applies the powerup to the player.
    /// </summary>
    /// <param name="player">Player GameObject</param>
    void Apply(GameObject player);
}

/// <summary>
/// Interfaces for Powerups with Limited Duration
/// </summary>
public interface IDurationPowerup{

    /// <summary>
    /// Ability part that lasts temporarily. Use StartCoroutine to take effect.
    /// </summary>
    IEnumerator AbilityEffect();
}

