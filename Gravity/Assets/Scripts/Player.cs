using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public CharacterController2D controller;

	const float Speed = 40f;

	float 	moveX 		= 0f;
	float 	moveY 		= 0f;
	bool 	jump 		= false;
	bool 	gChange 	= false;
	bool 	moveVer 	= false;

	void Update()
	{
		if (Input.GetButton("Change Gravity"))  gChange = true; // TODO: add bullet time

		// Change gravity
		if (gChange) {
			if (Input.GetKeyUp(KeyCode.W)) moveVer = controller.ChangeGravity(Vector2.up);
			if (Input.GetKeyUp(KeyCode.S)) moveVer = controller.ChangeGravity(Vector2.down);
			if (Input.GetKeyUp(KeyCode.A)) moveVer = controller.ChangeGravity(Vector2.left);
			if (Input.GetKeyUp(KeyCode.D)) moveVer = controller.ChangeGravity(Vector2.right);

			return;
		}
		
		// Walk
		if (!moveVer)
		{
			moveX = Input.GetAxisRaw("Horizontal") 	* Speed;
			moveY = Input.GetAxisRaw("Vertical") 	* Speed;
		} else
		{
			moveX = Input.GetAxisRaw("Vertical") 	* Speed;
			moveY = Input.GetAxisRaw("Horizontal") 	* Speed;
		}

		// Jump
		if (Input.GetButtonDown("Jump")) jump = true;
	}

	void FixedUpdate()
	{
		controller.Move(moveX * Time.fixedDeltaTime, jump);
		
		jump 	= false;
		gChange = false;
	}
}
