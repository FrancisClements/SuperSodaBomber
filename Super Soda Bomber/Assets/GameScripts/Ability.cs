using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
    Player Ability
        Contains each of the Player's Abilities
            Active
                Double Jump
                Dash
            Passive
                Long Jump
*/

public class Ability: MonoBehaviour { }

/// <summary>
/// Abilities that require player's action in order to activate it.
/// </summary>
public abstract class ActiveAbility : Ability
{
    /// <summary>
    /// Initializes the ability
    /// </summary>
    public virtual void Init(UnityEvent<Rigidbody2D> e)
    {
        e.AddListener(CallAbility);
    }

    /// <summary>
    /// Calls the ability and applies it to the player
    /// </summary>
    /// <param name="rigid">RigidBody of the player</param>
    public virtual void CallAbility(Rigidbody2D rigid) { }
}


/// <summary>
/// Abilities that enhances one of the player's abilities permanently.
/// </summary>
public abstract class PassiveAbility: Ability
{
    /// <summary>Multiplier of the ability/variable</summary>
    protected float multiplier = 2f;

    /// <summary>Updates the value which is implemented by the passive ability.
    /// </summary>
    /// <param name="oldValue">Variable being implemented/updated</param>
    /// <returns>Updated value</returns>
    public virtual float ApplyPassiveAbility(float oldValue)
    {
        return oldValue * multiplier;
    }
}


/// <summary>
/// Abilities that enhances one of the player's abilities temporarily.
/// </summary>
public abstract class Powerup: PassiveAbility
{
    /// <summary>Time until the ability/effect rans out</summary>
    protected float abilityDuration;
    protected float oldValue;

    /// <summary>Updates the value which is implemented by the passive ability.
    /// </summary>
    /// <param name="oldValue">variable being implemented</param>
    /// <returns>Updated value</returns>
    public override float ApplyPassiveAbility(float oldValue)
    {
        this.oldValue = oldValue;
        return base.ApplyPassiveAbility(oldValue);
    }

    /// <summary>Reverts the current value, removing the ability effect.</summary>
    public virtual float UnapplyPassiveAbility()
    {
        return oldValue;
    }

    /// <summary>Waits for duration and then unapply the ability.</summary>
    public virtual IEnumerator WaitAbilityEffect()
    {
        yield return new WaitForSeconds(abilityDuration);
        UnapplyPassiveAbility();
    }
}


//ABILITY TYPES (ACTIVE)

/*
    Double Jump
        Lets the player jump mid-air by pressing
        jump button
*/

public class DoubleJump : ActiveAbility
{
    private float jumpForce = 400f;
    private float jumpMultiplier = 1.25f;
    public override void CallAbility(Rigidbody2D rigid)
    {
        rigid.AddForce(new Vector2(0f, jumpForce * jumpMultiplier));
    }
}

/*
    Dash
        Lets the player move quickly by pressing
        attack button twice
*/

public class Dash : ActiveAbility
{
    private float dashForce = 15f;
    public override void CallAbility(Rigidbody2D rigid)
    {
        rigid.velocity += new Vector2(dashForce, 0);
    }
}


//ABILITY TYPES (PASSIVE)

/*
    Long Jump
        Lets the player jump higher.
*/

public class LongJump : PassiveAbility
{
    void Awake(){
        multiplier = 1.5f;
    }
}
