using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] CharacterController2D 	controller;

	const float Speed = 40f;

	bool 	jump 		= false;
	bool 	gChange 	= false;
	bool 	moveVer 	= false;
	float 	moveX 		= 0f;

	void Start()
	{
		controller.Spawn();
	}

	void Update()
	{
		if (Input.GetButton("Change Gravity"))  gChange = true; // TODO: add bullet time

		// Change gravity
		if (gChange) {
			if (Input.GetKey(KeyCode.W)) moveVer = controller.ChangeGravity(Vector2.up);
			if (Input.GetKey(KeyCode.S)) moveVer = controller.ChangeGravity(Vector2.down);
			if (Input.GetKey(KeyCode.A)) moveVer = controller.ChangeGravity(Vector2.left);
			if (Input.GetKey(KeyCode.D)) moveVer = controller.ChangeGravity(Vector2.right);

			return;
		}
		
		// Walk
		if (!moveVer) 	moveX = Input.GetAxisRaw("Horizontal") 	* Speed;
		else 			moveX = Input.GetAxisRaw("Vertical") 	* Speed;

		// Jump
		if (Input.GetButtonDown("Jump")) 	jump = true;
		if (Input.GetButtonUp("Jump")) 		jump = false;
	}

	void FixedUpdate()
	{
		controller.Move(moveX * Time.fixedDeltaTime, jump);
		
		gChange = false;
	}
}
