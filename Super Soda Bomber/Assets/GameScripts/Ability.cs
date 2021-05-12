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

/// <summary>
/// Empty Base Class Ability.
/// </summary>
public class Ability
{
    /// <summary>
    /// Initializes the ability
    /// </summary>
    public virtual void Init(UnityEvent<Rigidbody2D> e) {}
}
/// 
/// <summary>
/// Abilities that require player's action in order to activate it.
/// </summary>
public abstract class ActiveAbility : Ability
{
    public float cooldown { get; protected set; }

    /// <summary>
    /// Initializes the ability's UnityEvent.
    /// </summary>
    /// <param name="e"></param>
    public override void Init(UnityEvent<Rigidbody2D> e)
    {
        e.AddListener(CallAbility);
    }

    /// <summary>
    /// Calls the ability and applies it to the player
    /// </summary>
    /// <param name="rigid">RigidBody of the player</param>
    public abstract void CallAbility(Rigidbody2D rigid);
    public virtual void OnFlip(){}
}

/// <summary>
/// Abilities that enhances one of the player's abilities permanently.
/// </summary>
public abstract class PassiveAbility: Ability
{
    //Multiplier of the ability/variable
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

//ABILITY TYPES (ACTIVE)

/*
    Double Jump
        Lets the player jump mid-air by pressing
        jump button
*/

public class DoubleJump : ActiveAbility
{
    private float jumpForce = 300f;
    private float jumpMultiplier = 1.25f;

    public override void CallAbility(Rigidbody2D rigid)
    {
        rigid.AddForce(new Vector2(0f, jumpForce * jumpMultiplier));
    }
}

/*
    Dash
        Lets the player move quickly by tapping the
        joystick twice
*/

public class Dash : ActiveAbility
{
    private float dashForce = 15f;
    public GameObject fx = Resources.Load("Prefabs/Particles/DashParticle") as GameObject;

    public Dash(){
        cooldown = 3f;
    }

    public override void Init(UnityEvent<Rigidbody2D> e)
    {
        base.Init(e);
        e.AddListener((rb2d) => CallCooldownUI());
    }
    
    public override void CallAbility(Rigidbody2D rigid)
    {
        rigid.velocity += new Vector2(dashForce, 0);
    }

    private void CallCooldownUI(){
        //calls the UI
        UICooldownDebug.current.CallCooldownUI(
            name: this.GetType().FullName,
            location: "button", duration: cooldown);
    }

    public override void OnFlip(){
        dashForce *= -1;
    }
}


//ABILITY TYPES (PASSIVE)

/*
    Long Jump
        Lets the player jump higher.
*/

public class LongJump : PassiveAbility
{
   public LongJump(){
       multiplier = 1.25f;
   }
}
