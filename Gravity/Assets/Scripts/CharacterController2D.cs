using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] [Range(0, .3f)] 	float 		movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] 					LayerMask 	whatIsGround;				// A mask determining what is ground to the character
	[SerializeField] 					Transform 	checkGround;				// A position marking where to check if the player is grounded.
	[SerializeField] 					Transform 	checkCeiling;				// A position marking where to check for ceilings
	[SerializeField]					Transform 	checkPoint;

	#region Consts
		const float RadiusGrounded 	= .2f; 		// Radius of the overlap circle to determine if grounded
		const float RadiusCeiling 	= .2f; 		// Radius of the overlap circle to determine if the player can stand up
		const float jumpForce 		= 16f;		// Amount of force added when the player jumps.
		const float GForce 			= 13f;		// Gravity force
		const float VelNormal 		= 10f;
		const float VelBulletTime 	= 2f;
	#endregion
	
	#region Vars
		bool 		isGrounded;		// Whether or not the player is grounded.
		bool 		isDead 		= false;
		Vector2 	jumpDir 	= Vector2.up * jumpForce;
		Vector2 	dirGravity 	= Vector2.down;
		Vector3 	moveVel		= Vector3.zero;
		Rigidbody2D rb;
	#endregion

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		if (isDead) return;

		bool wasGrounded 	= isGrounded;
		isGrounded 			= false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(checkGround.position, RadiusGrounded, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			GameObject temp = colliders[i].gameObject;
			if (temp == gameObject) continue; // In case it's itself
			
			// Damage layer
			if (temp.layer == 10)
			{
				Spawn();
				return;
			}

			isGrounded = true;
		}
	}

	public void Move(float move, bool jump)
	{
		// Gravity
		rb.velocity += GForce * rb.mass * dirGravity * Time.fixedDeltaTime;

		// Walk
		if (dirGravity.y != 0) 	rb.velocity = new Vector2(move * VelNormal	, rb.velocity.y);
		else 					rb.velocity = new Vector2(rb.velocity.x		, move * VelNormal);

		Jump(jump);
	}

	void Jump(bool jump)
	{
		if (isGrounded && jump)
		{
			isGrounded = false;
			rb.velocity += jumpDir;
			return;
		}
		
		// TODO: improve
		// The longer holds the button, higher the jump
		if (rb.velocity.y > 0 && jump) rb.velocity += jumpDir * Time.fixedDeltaTime;
	}

	public bool ChangeGravity(Vector2 dir)
	{
		// Rotation
		float rotAngle;
		if (dir.x != 0f) 	rotAngle = dir.x * 90f;
		else 				rotAngle = dir.y > 0f ? 180f : 0f;
		transform.rotation = Quaternion.Euler(0f, 0f, rotAngle);

		dirGravity 	= dir; 					// Gravity direction
		jumpDir 	= -dir * jumpForce; 	// Jump direction		

		return dir.y == 0f;
	}

	public void Spawn()
	{
		rb.position = checkPoint.position;
	}
}