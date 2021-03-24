using System.Collections;
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
		{"RUN", "fizzy_run"},
		{"READY_JUMP", "fizzy_readyJump"},
		{"JUMP", "fizzy_jump"},
		{"FALL", "fizzy_fall"},
		/*
		{"D_JUMP", "fizzy_double_jump"},
		{"LAND", "fizzy_land"},
		{"THROW", "fizzy_throw"},
		{"FIRE", "fizzy_fire_gun"},
		{"DASH", "fizzy_dash"},
		*/
	};

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
	/// Changes Player's animation
	/// </summary>
	/// <param name="name">animation name (uppercase)</param>
	public void ChangeAnimState(string name){
        name = name.ToUpper();
		AnimatorStateInfo currentAnim = animator.GetCurrentAnimatorStateInfo(0);

		//prevents the animator to play same state all the time
		if (currentAnim.IsName(ANIM[name])) return;
		animator.Play(ANIM[name]);

	}
}
