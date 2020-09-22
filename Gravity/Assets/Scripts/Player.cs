using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	#region Serialized
		[SerializeField] LayerMask 	whatIsGround;	// A mask determining what is ground to the character
		[SerializeField] Transform 	checkGround;	// A position marking where to check if the player is grounded.
		[SerializeField] Transform 	checkPoint;
	#endregion

	#region Consts
		const float SpeedRun 		= 10f;
		const float SpeedMax 		= .05f;
		const float SpeedDec 		= .03f;
		const float JumpForce 		= 1850f;
		const float GForce 			= 13f;
		const float VelBulletTime 	= 2f;
	#endregion

	#region Vars
		bool 			isGrounded	= false;
		bool 			isDead 		= false;
		bool 			isJumping 	= false;
		bool 			isGChange 	= false;
		bool 			isMoveVer 	= false; 	// Whether the player movement is in the X or Y axis 
		float 			moveX 		= 0f;
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
		if (isGChange) {
			if (Input.GetKey(KeyCode.W)) ChangeGravity(Vector2.up);
			if (Input.GetKey(KeyCode.S)) ChangeGravity(Vector2.down);
			if (Input.GetKey(KeyCode.A)) ChangeGravity(Vector2.left);
			if (Input.GetKey(KeyCode.D)) ChangeGravity(Vector2.right);

			return;
		}
		
		// Walk
		if (!isMoveVer) moveX = Input.GetAxisRaw("Horizontal") 	* SpeedRun;
		else 			moveX = Input.GetAxisRaw("Vertical") 	* SpeedRun;

		// Jump
		if (Input.GetButtonDown("Jump")) 	isJumping = true;
		if (Input.GetButtonUp("Jump")) 		isJumping = false;
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

	void Move()
	{
		// Axis
		Vector2 velTarget;
		if (!isMoveVer) velTarget = new Vector2(moveX			, rb.velocity.y);
		else 			velTarget = new Vector2(rb.velocity.x	, moveX);

		// Acceleration / Deacceleration
		Vector2 velCur 		= Vector2.zero;
		float smoothTime 	= velTarget == Vector2.zero ? SpeedDec : SpeedMax;
		rb.velocity 		= Vector2.SmoothDamp(rb.velocity, velTarget, ref velCur, smoothTime);
	}

	void Jump()
	{
		if (!isGrounded && IsFalling()) { isJumping = false; return; }

		// TODO: jump is stronger at the X axis
		// The longer holds the button, higher the jump
		if 		(isGrounded && isJumping) 	rb.AddForce(jumpDir);
		else if (!isJumping) 				rb.AddForce(-jumpDir / 4);
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
	}

	bool IsFalling()
	{
		if 		(dirGravity == Vector2.down) 	return rb.velocity.y < 0;
		else if (dirGravity == Vector2.up) 		return rb.velocity.y > 0;
		else if (dirGravity == Vector2.right) 	return rb.velocity.x > 0;
		return rb.velocity.x < 0;
	}

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

	#region OnCollision

		void Collision()
		{
			Collider2D col 	= Physics2D.OverlapBox(checkGround.transform.position, new Vector2(.7f, .2f), 0f, whatIsGround);
			isGrounded 		= col != null;
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
