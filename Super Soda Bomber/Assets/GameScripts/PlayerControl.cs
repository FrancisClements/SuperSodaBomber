using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
PlayerMovement
	ovesees the controllers of the player 
	and interprets it to the PlayerControl & PlayerAttack Script
*/
public class PlayerControl : MonoBehaviour {


	//importing scripts
	public PlayerMovement controller;
	public PlayerAttack attacker;

	//Variables
	float horizontalMove = 0f;
	public Joystick joystick;
	public float runSpeed = 40f;
	bool jump = false;
	bool attack = false;

	//Jump Button
	public void PressJump(){
		jump = true;
	}

	//Attack Button
	public void PressAttack(){
		attack = true;
	}



	void Update () {

		//movement
		if (joystick.Horizontal >= .5f || Input.GetAxisRaw("Horizontal") > 0){
			horizontalMove = runSpeed;
		}
		else if (joystick.Horizontal <= -.5f || Input.GetAxisRaw("Horizontal") < 0){
			horizontalMove = -runSpeed;
		}
		else{
			horizontalMove = 0;
		}

		//keypress jump
		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
		}

		//keypress shoot
		if (Input.GetButtonDown("Fire2")){
			attack = true;
		}

	}

	void FixedUpdate ()
	{
		// interprets the controls to the script
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
		attacker.Attack(horizontalMove != 0, attack);
		jump = false;
		attack = false;
	}
}
