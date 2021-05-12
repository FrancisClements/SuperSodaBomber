using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/*
PlayerControl
	Used to get the physics and the overall movement of the player
	such as walking and jumping
*/

public class PlayerMovement : PublicScripts
{

	[Header("Variables")]
	[Space]

	public float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	private float m_readyJumpSpeed = .15f;
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .75f;  // How much to smooth out the movement
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform floor;									// A position where double jump particle spawns.

	const float k_GroundedRadius = .15f; // Radius of the overlap circle to determine if grounded
	const float gracePeriod = .5f; // Time when player can jump regardless of groundcheck
	private int health = 3; // Health of the player

	private float hangTime = 0f;
	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;
	private CheckpointScript checkScript;

	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_hangJump = false;    // If player is eligible to perform a hangjump (Coyote Time)
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private bool m_doubleJump = true;

	public static Vector3 playerPos { get; private set; }

	//animator
	private PlayerAnimation animator = PlayerAnimation.current;

	[Header("Events")]
	[Space]

	//this will be used on sounds, traps, etc.
	public UnityEvent OnLandEvent;

	//this will be used on abilities
	public PlayerAbilities chosenAbility;
	private AbilityVerifier a_Verifier;

	public delegate void flipDelegate();

    /// <summary>
    /// Event when the player flips. (Dash)
    /// </summary>
    public event flipDelegate flipEvent;		

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> {}

	void Awake()
	{
		//config variables
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		//config ability and the verifier
		a_Verifier = gameObject.AddComponent<AbilityVerifier>();
		AbilityProcessor.Fetch(chosenAbility, this);

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

	}

	void Start(){
		a_Verifier.Init(m_Rigidbody2D, chosenAbility);
		GameplayScript.SetHpUI(health);
	}

	void FixedUpdate()
	{
		//Updates the player tranform
		playerPos = transform.position;
		//Player Animation script
		animator = PlayerAnimation.current;
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

        //add hangtime to jump
        if (colliders.Length == 0 && Time.time < hangTime && !m_hangJump){
            m_hangJump = true;
            m_Grounded = true;
            if (!wasGrounded)
                OnLandEvent.Invoke();
        }

		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject) 
			{
                m_hangJump = false;
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
                hangTime = gracePeriod + Time.time;
			}
		}

		a_Verifier.UpdateTransform(floor);
	}

	public void Move(float move, bool readyJump)
	{
		//if the player is ready to jump, decrease the speed
		if (readyJump)
			move *= m_readyJumpSpeed;

		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

		// If the input is moving the player right and the player is facing left...
		if ((move > 0 && !m_FacingRight) || (move < 0 && m_FacingRight))
		{
			// ... flip the player.
			Flip();
		}
	}

	public void Jump(bool jump){
		// If the player should jump...
		if ((m_Grounded||m_hangJump) && jump)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_hangJump? m_JumpForce*1.25f : m_JumpForce));
			m_Grounded = false;
            m_hangJump = false;
			hangTime = Time.time;

			//prevents user from double jumping indefinely midair
			m_doubleJump = true;
		}

		//verifier for double jump
		else if (m_doubleJump && jump)
        {
			a_Verifier.Verify(m_Grounded, jump);
			m_doubleJump = false;
        }
	}

	public void DoubleTap(bool _doubleTap){
		a_Verifier.Verify(_doubleTap);
	}

	public void ManageAnim(float move, bool readyJump){
		if (readyJump && m_Grounded){
			animator.ChangeAnimState("READY_JUMP");
			return;
		}
		
		if (!m_Grounded){
			if (m_Rigidbody2D.velocity.y > 0){
			animator.ChangeAnimState("JUMP");
			}
			else if(m_Rigidbody2D.velocity.y < 0){
				animator.ChangeAnimState("FALL");
			}
		}
		else {
			if ((Mathf.Abs(move) == PlayerControl.GetRunSpeed())){
				animator.ChangeAnimState("RUN");
			}
			else if (move != 0){
				animator.ChangeAnimState("WALK");
			}
			else{
				animator.ChangeAnimState("IDLE");
			}
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		transform.Rotate(0f,180f,0);
		a_Verifier.SetFlip(m_FacingRight);

		//calls the flip event
		flipEvent?.Invoke();
	}
}

/// <summary>
/// Verifies the use of active abilities on runtime.
/// </summary>
public class AbilityVerifier: PublicScripts{
	private PlayerAbilities chosenAbility;
	private Dictionary<PlayerAbilities, GameObject> fxDict = new Dictionary<PlayerAbilities, GameObject>();
	private Rigidbody2D rigid;
	private bool ready = true;
	private Transform floorSource;
	private bool isFacingRight = true;

	//asynchronous work
	private Coroutine coro;

	public void Init(Rigidbody2D r, PlayerAbilities a){
		chosenAbility = a;
		rigid = r;

		//add the fx to dictionary if it has one
		if (chosenAbility.HasFlag(PlayerAbilities.DoubleJump)){
			var fx = Resources.Load("Prefabs/Particles/DoubleJumpParticle") as GameObject;
			fxDict.Add(PlayerAbilities.DoubleJump, fx);
		}
		if (chosenAbility.HasFlag(PlayerAbilities.Dash)){
			var fx = Resources.Load("Prefabs/Particles/DashParticle") as GameObject;
			fxDict.Add(PlayerAbilities.Dash, fx);
		}
	}

	public void UpdateTransform(Transform floor){
		floorSource = floor;
	}

	public void SetFlip(bool right){
		isFacingRight = right;
	}
	
	//Only one of these Verify() works because of the chosen ability
	
	/// <summary>
    /// Ability Verifier for Dash
    /// </summary>
    public void Verify(bool doubleTap){
        if (chosenAbility.HasFlag(PlayerAbilities.Dash) && 
			doubleTap && ready){
				InvokeAbility(PlayerAbilities.Dash);
				var particle = Instantiate(fxDict[PlayerAbilities.Dash], PlayerMovement.playerPos, Quaternion.identity, transform);
				if(!isFacingRight){
					particle.transform.localScale = new Vector3(-1f, 1f, 1f);
				}
		}
            
    }

    /// <summary>
    /// Ability Verifier for Double Jump
    /// </summary>
    public void Verify(bool grounded, bool jumped){
        if (chosenAbility.HasFlag(PlayerAbilities.DoubleJump) &&
			!grounded && jumped){
				InvokeAbility(PlayerAbilities.DoubleJump);
				Instantiate(fxDict[PlayerAbilities.DoubleJump], floorSource.position, Quaternion.identity);
		}
    }

	private void InvokeAbility(PlayerAbilities ability){
		//add score
		GameplayScript.current.AddScore(scores["ability"]);

		float cooldown = AbilityProcessor.GetCooldown(ability);
		AbilityProcessor.CallEvent(ability, rigid);
		coro = StartCoroutine(AbilityCooldown(cooldown));

	}

	private IEnumerator AbilityCooldown(float secs){
		ready = false;
		yield return new WaitForSeconds(secs);
		ready = true;
	}
}