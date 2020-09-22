using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Serialized
		[SerializeField] LayerMask 	whatIsGround;	// A mask determining what is ground to the character
		[SerializeField] Transform 	checkGround;	// A position marking where to check if the player is grounded.
		[SerializeField] Transform 	checkCeiling;	// A position marking where to check for ceilings
		[SerializeField] Transform 	checkPoint;
	#endregion

	#region Consts
		const float SpeedRun 		= 10f;
		const float JumpForce 		= 16f;
		const float GForce 			= 13f;
		const float VelBulletTime 	= 2f;
	#endregion

	#region Vars
		bool 		isGrounded	= false;	// Whether or not the player is grounded
		bool 		isDead 		= false;	
		bool 		jump 		= false;
		bool 		gChange 	= false;
		bool 		moveVer 	= false;
		float 		moveX 		= 0f;
		Vector2 	jumpDir 	= Vector2.up * JumpForce;
		Vector2 	dirGravity 	= Vector2.down;
		Vector3 	moveVel		= Vector3.zero;
		Rigidbody2D rb;
	#endregion

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		Spawn();
	}

	void Update()
	{
		if (Input.GetButton("Change Gravity")) gChange = true; // TODO: add bullet time

		// Change gravity
		if (gChange) {
			if (Input.GetKey(KeyCode.W)) ChangeGravity(Vector2.up);
			if (Input.GetKey(KeyCode.S)) ChangeGravity(Vector2.down);
			if (Input.GetKey(KeyCode.A)) ChangeGravity(Vector2.left);
			if (Input.GetKey(KeyCode.D)) ChangeGravity(Vector2.right);

			return;
		}
		
		// Walk
		if (!moveVer) 	moveX = Input.GetAxisRaw("Horizontal") 	* SpeedRun;
		else 			moveX = Input.GetAxisRaw("Vertical") 	* SpeedRun;

		// Jump
		if (Input.GetButtonDown("Jump")) 	jump = true;
		if (Input.GetButtonUp("Jump")) 		jump = false;
	}

	void FixedUpdate()
	{
		if (isDead) return;
		
		// Gravity
		rb.velocity += GForce * rb.mass * dirGravity * Time.fixedDeltaTime;

		Move();
		Jump();
		gChange = false;
	}

	void Move()
	{
		// Walk
		if (dirGravity.y != 0) 	rb.velocity = new Vector2(moveX			, rb.velocity.y);
		else 					rb.velocity = new Vector2(rb.velocity.x	, moveX);

		// TODO: apply acceleration over small amout of time
	}

	void Jump()
	{
		if (isGrounded && jump)
		{
			rb.velocity += jumpDir;
			return;
		}
		
		// TODO: improve
		// The longer holds the button, higher the jump
		if (rb.velocity.y > 0 && jump) rb.velocity += jumpDir * Time.fixedDeltaTime;
	}

	void ChangeGravity(Vector2 dir)
	{
		// Rotation
		float rotAngle;
		if (dir.x != 0f) 	rotAngle = dir.x * 90f;
		else 				rotAngle = dir.y > 0f ? 180f : 0f;
		transform.rotation = Quaternion.Euler(0f, 0f, rotAngle);

		dirGravity 	= dir; 					// Gravity direction
		jumpDir 	= -dir * JumpForce; 	// Jump direction		
		moveVer 	= dir.y == 0f; 			// Vertical move
	}

	void Spawn()
	{
		// TODO: transition
		rb.position = checkPoint.position;
		ChangeGravity(Vector2.down);
		isGrounded 	= true;
	}

	#region OnCollision
		void OnCollisionEnter2D(Collision2D col)
		{
			switch(col.gameObject.layer)
			{
				case 8: 	isGrounded = true; 	return;
				case 10: 	Spawn(); 			return;
			}
		}

		void onCollisionStay2D(Collision2D col)
		{
			switch(col.gameObject.layer)
			{
				case 8: isGrounded = true; return;
			}
		}

		void OnCollisionExit2D(Collision2D col)
		{
			switch(col.gameObject.layer)
			{
				case 8: isGrounded = false; return;
			}
		}
	#endregion
}
