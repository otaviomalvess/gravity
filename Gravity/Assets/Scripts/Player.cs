﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Serialized
		[SerializeField] LayerMask 	whatIsGround;	// A mask determining what is ground to the character
		[SerializeField] Transform 	checkGround;	// A position marking where to check if the player is grounded.
		[SerializeField] Transform 	checkPoint;
		[SerializeField] Vector2 	test; 			// Variable for tests
	#endregion

	#region Consts
		const float SpeedRun 		= 10f;
		const float JumpForce 		= 3200f;
		const float GForce 			= 13f;
		const float VelBulletTime 	= 2f;
		const float FallLimit 		= 11f;
	#endregion

	#region Vars
		bool 			isGrounded	= false;
		bool 			isDead 		= false;
		bool 			isJumping 	= false;
		bool 			isHelding 	= false;
		bool 			isGChange 	= false;
		bool 			isMoveVer 	= false; 	// Whether the player movement is in the X or Y axis
		float 			run 		= 0f;
		Vector2 		jumpDir 	= Vector2.up * JumpForce;
		Vector2 		dirGravity 	= Vector2.down;
		Rigidbody2D 	rb;
		SpriteRenderer 	sr;
	#endregion

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	void Start()
	{
		Spawn();
	}

	void Update()
	{
		if (Input.GetButton("Change Gravity")) isGChange = true; // TODO: add bullet time

		// Change gravity
		if (isGChange)
		{
			if (Input.GetKey(KeyCode.W)) ChangeGravity(Vector2.up);
			if (Input.GetKey(KeyCode.S)) ChangeGravity(Vector2.down);
			if (Input.GetKey(KeyCode.A)) ChangeGravity(Vector2.left);
			if (Input.GetKey(KeyCode.D)) ChangeGravity(Vector2.right);

			return;
		}
		
		// Walk
		if (!isMoveVer) run = Input.GetAxisRaw("Horizontal") 	* SpeedRun;
		else 			run = Input.GetAxisRaw("Vertical") 		* SpeedRun;

		// Jump
		if (Input.GetButtonDown("Jump") && isGrounded) 	isJumping = true;
		if (Input.GetButtonUp("Jump")) 					JumpCut();
	}

	void FixedUpdate()
	{
		if (isDead) return;
		
		// Gravity
		rb.velocity += GForce * rb.mass * dirGravity * Time.fixedDeltaTime;

		Collision();
		Move();
		Jump();
		isGChange = false;
	}

	void ChangeGravity(Vector2 dir)
	{
		// Rotation
		float rotAngle;
		if (dir.x != 0f) 	rotAngle = dir.x * 90f;
		else 				rotAngle = dir.y > 0f ? 180f : 0f;

		transform.rotation = Quaternion.Euler(0f, 0f, rotAngle); // TODO: smooth rotation

		dirGravity 	= dir; 					// Gravity direction
		jumpDir 	= -dir * JumpForce; 	// Jump direction
		isMoveVer 	= dir.y == 0f; 			// Vertical move
		isGChange 	= false;
	}

	// TODO: player not moving on air
	void Move()
	{
		// Axis
		Vector2 velTarget;
		if (!isMoveVer) velTarget = new Vector2(run				, rb.velocity.y);
		else 			velTarget = new Vector2(rb.velocity.x	, run);

		// Acceleration / Deacceleration
		Vector2 velCur 		= rb.velocity;
		float 	smoothTime 	= run == 0f ? .03f : .05f;

		rb.velocity = Vector2.SmoothDamp(rb.velocity, velTarget, ref velCur, smoothTime);
	}

	#region Jump
		void Jump()
		{
			if (!isJumping) return;

			rb.AddForce(jumpDir);
			isJumping = false;
		}

		void JumpCut()
		{
			// TODO: when button is released near complete stop in air, the force is added to the gravity
			
			rb.AddForce(-jumpDir / 3);
		}

		bool IsFalling()
		{
			if 		(dirGravity == Vector2.down) 	return rb.velocity.y < 0;
			else if (dirGravity == Vector2.up) 		return rb.velocity.y > 0;
			else if (dirGravity == Vector2.right) 	return rb.velocity.x > 0;
			return rb.velocity.x < 0;
		}
	#endregion

	#region (Re)Spawn
		void Respawn()
		{
			isDead 		= true;
			sr.enabled 	= false;
			
			StartCoroutine(Transition());
		}

		IEnumerator Transition()
		{
			yield return new WaitForSeconds(2);
			Spawn();
		}

		void Spawn()
		{
			rb.position = checkPoint.position;
			ChangeGravity(Vector2.down);
			sr.enabled 	= true;
			isDead 		= false;
		}
	#endregion

	#region Collision

		void Collision()
		{
			Collider2D col 	= Physics2D.OverlapBox(checkGround.transform.position, new Vector2(transform.localScale.x, .2f), 0f, whatIsGround);
			isGrounded 		= col != null;
			// TODO: draw collider
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			switch(col.gameObject.layer)
			{
				case 10: Respawn(); return;
			}
		}
	#endregion
}
