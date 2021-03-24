using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
PlayerControl
	Used to get the physics and the overall movement of the player
	such as walking and jumping
*/

public class PlayerMovement : PublicScripts
{

	[Header("Variables")]
	[Space]

	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .75f;  // How much to smooth out the movement
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.

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

	//animator
	private PlayerAnimation animator = PlayerAnimation.current;

	//this will be used for solely on jump anticipation
	public DetectButtonPress buttonDetector;

	[Header("Events")]
	[Space]

	//this will be used on sounds, traps, etc.
	public UnityEvent OnLandEvent;

	//this will be used on abilities
	public PlayerAbilities chosenAbility;
	public AbilityEvent m_AbilityEvent;

	[System.Serializable]
	public class AbilityEvent: UnityEvent<Rigidbody2D>{}			

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> {}

	void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		ConfigureAbility();


		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (m_AbilityEvent == null) {
			m_AbilityEvent = new AbilityEvent();
		}

	}

	void Start()
    {
	}

	void ConfigureAbility()
    {
		ActiveAbility a;
		Ability ability = AbilityProcessor.Fetch(chosenAbility);
		Debug.Log($"ability: {ability.GetType()}");

		if (typeof(ActiveAbility).IsAssignableFrom(ability.GetType())){
			a = ability as ActiveAbility;
			m_AbilityEvent.AddListener(a.CallAbility);
			Debug.Log("the ability inherits ActiveAbility");
        }
		else if (typeof(PassiveAbility).IsAssignableFrom(ability.GetType())){
			Debug.Log("the ability inherits Passive Ability");
        }
        else
        {
			Debug.Log("the ability does not inherit a particular ability");
        }

    }


	void FixedUpdate()
	{
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
	}

	public void Move(float move, bool jump)
	{
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

		// If the player should jump...
		if ((m_Grounded||m_hangJump) && jump)
		{
			// Add a vertical force to the player.
			m_Rigidbody2D.AddForce(new Vector2(0f, m_hangJump? m_JumpForce*1.25f : m_JumpForce));
			m_Grounded = false;
            m_hangJump = false;
			hangTime = Time.time;

			// Add score
			GameplayScript.current.AddScore(scores["jump"]);
		}
		ManageAnim(move);
	}

	private void ManageAnim(float move){
		if (buttonDetector.getPressedStatus() && m_Grounded){
			animator.ChangeAnimState("READY_JUMP");
			return;
		}

		if (m_Rigidbody2D.velocity.y > 0 && !m_Grounded){
			animator.ChangeAnimState("JUMP");
		}
		else if(m_Rigidbody2D.velocity.y < 0 && !m_Grounded){
			animator.ChangeAnimState("FALL");
		}
		else if (move != 0 && m_Grounded){
			animator.ChangeAnimState("RUN");
		}
		else if (m_Grounded){
			animator.ChangeAnimState("IDLE");
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;
		transform.Rotate(0f,180f,0);
	}

	// triggers when player touches the checkpoint
    private void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.layer == 10){
			//gets SpriteRenderer and changes the image
			checkScript = col.GetComponent<CheckpointScript>();

			//activate these scripts if the checkpoint was not saved yet
			if(!checkScript.isTouched){
				checkScript.ChangeState();
				GameplayScript.current.AddScore(scores["checkpoint"]);
				GameplayScript.current.SetCheckpoint(col.transform.position, col.name);
				StartCoroutine(checkScript.Notify());
				Debug.Log("Checkpoint Saved!");
			}
		}
    }

}