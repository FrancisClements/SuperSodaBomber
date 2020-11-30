﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour {


	//importing scripts
	public PlayerControl controller;

	//Variables
	float horizontalMove = 0f;
	public Joystick joystick;
	public float runSpeed = 40f;
	bool jump = false;

	//OnPress of Jump Button
	public void PressJump(){
		jump = true;
	}
	
	// Update is called once per frame
	void Update () {


		if (joystick.Horizontal >= .5f){
			horizontalMove = runSpeed;
		}
		else if (joystick.Horizontal <= -.5f){
			horizontalMove = -runSpeed;
		}
		else{
			horizontalMove = 0;
		}

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
		}

	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
		jump = false;
	}
}
