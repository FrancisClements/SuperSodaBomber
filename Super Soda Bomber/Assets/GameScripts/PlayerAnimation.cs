using System.Collections.Generic;
using UnityEngine;

/*
	Player Animation
*/
/// <summary>
/// Manages the animation fo Fizzy and
/// changes its state.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    //animation states
	private Dictionary<string, string> ANIM = new Dictionary<string, string>(){
		{"IDLE", "fizzy_idle"},
		{"WALK", "fizzy_walk"},
		{"RUN", "fizzy_run"},
		{"READY_JUMP", "fizzy_readyJump"},
		{"JUMP", "fizzy_jump"},
		{"FALL", "fizzy_fall"},
		{"THROW", "fizzy_throw"},
		{"FIRE_FIZZTOL", "fizzy_fireFizztol"},
		{"FIRE_SHOTGUN", "fizzy_fireShotgun"},
		/*
		{"D_JUMP", "fizzy_double_jump"},
		{"LAND", "fizzy_land"},
		{"DASH", "fizzy_dash"}
		*/
	};

	//this is used stop other animations from interrupting it
	private bool isAnimInterruptable = true;

	private Animator animator;

    //set the static class
    public static PlayerAnimation current;

    void Awake(){
		animator = gameObject.GetComponent<Animator>();
    }

    //creates a static reference for PlayerAnimation
    void Start(){
        current = this;   
    }

	/// <summary>
	/// Sets current animation's interruptable status
	/// </summary>
	/// <param name="id">0 = no, 1 = yes</param>
	public void SetAnimInterruptable(int id){
		isAnimInterruptable = id == 1;
	}

	/// <summary>
	/// Changes Player's animation
	/// </summary>
	/// <param name="name">animation name (uppercase)</param>
	public void ChangeAnimState(string name){
        name = name.ToUpper();
		AnimatorStateInfo currentAnim = animator.GetCurrentAnimatorStateInfo(0);
 
		//prevents the animator to play same state all the time
		if (currentAnim.IsName(ANIM[name]) || !isAnimInterruptable || animator.runtimeAnimatorController == null) return;
		animator.Play(ANIM[name]);
	}

}
