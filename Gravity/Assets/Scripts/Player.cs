using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	#region Serialized
		[SerializeField] LayerMask 	whatIsGround;	// A mask determining what is ground to the character
		[SerializeField] Transform 	checkGround;	// A position marking where to check if the player is grounded
		[SerializeField] Transform 	camOffset;		// A position marking the camera offset
		[SerializeField] Transform 	checkPoint;
		[SerializeField] Vector2  	test; 			// Variable for tests
	#endregion

	#region Consts
		const float SpeedRun 		= 10f;
		const float JumpForce 		= 3200f;
		const float GForce 			= 13f;
		const float VelBulletTime 	= 2f;
		const float FallLimit 		= 20f;
	#endregion

	#region Vars
		bool 			gFirstTime 	= true;
		bool 			isGrounded	= false;
		bool 			wasGrounded = false;
		bool 			isDead 		= false;
		bool 			isJumping 	= false;
		bool 			jumpedOnce  = false;
		bool 			isHelding 	= false;
		bool 			canGChange 	= false;
		bool 			isMoveVer 	= false; 	// Whether the player movement is in the X or Y axis
		float 			run 		= 0f;
		Vector2 		dirGForce; 				// GForce * rb.mass * dirGravity
		Vector2 		jumpDir 	= Vector2.up * JumpForce;
		Vector2 		dirGravity 	= Vector2.down;
		Rigidbody2D 	rb;
		SpriteRenderer 	sr;
		Animator 		anim;
	#endregion

	#region Public
		public bool IsPaused = false;
	#endregion

	#region Events
		[Header("Events")]
		public UnityEvent OnRespawnEvent;
		public UnityEvent OnPause;
		public UnityEvent StartMusic;

		[System.Serializable]
		public class 	Vector2Event : UnityEvent<Vector2> {}
		public 			Vector2Event OnGravityChange;
	#endregion

	private void Awake()
	{
		rb 	 = GetComponent<Rigidbody2D>();
		sr 	 = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();

		if (OnRespawnEvent 	== null) OnRespawnEvent 	= new UnityEvent();
		if (OnPause 		== null) OnPause 			= new UnityEvent();
		if (StartMusic 		== null) StartMusic 		= new UnityEvent();
		if (OnGravityChange == null) OnGravityChange 	= new Vector2Event();
	}

	void Start()
	{
		Spawn();
	}

	void Update()
	{
		if (IsPaused) return;

		// Pause
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
			OnPause.Invoke();

		// Change gravity
		if (canGChange)
		{
			if (Input.GetKey(KeyCode.I)) ChangeGravity(Vector2.up);
			if (Input.GetKey(KeyCode.K)) ChangeGravity(Vector2.down);
			if (Input.GetKey(KeyCode.J)) ChangeGravity(Vector2.left);
			if (Input.GetKey(KeyCode.L)) ChangeGravity(Vector2.right);
		}
		
		// Walk
		if (!isMoveVer) run = Input.GetAxisRaw("Horizontal") 	* SpeedRun;
		else 			run = Input.GetAxisRaw("Vertical") 		* SpeedRun;

		// Jump
		if (Input.GetButtonDown("Jump") && isGrounded) isJumping = true;
		if (Input.GetButtonUp("Jump")) JumpCut();
	}

	void ChangeGravity(Vector2 dir)
	{
		// Rotation
		float rotAngle;
		if (dir.x != 0f) 	rotAngle = dir.x * 90f;
		else 				rotAngle = dir.y > 0f ? 180f : 0f;

		transform.rotation = Quaternion.Euler(0f, 0f, rotAngle); // TODO: smooth rotation

		dirGravity 	= dir; 						// Gravity direction
		dirGForce 	= GForce * rb.mass * dir;

		OnGravityChange.Invoke(dir);
		jumpDir 	= -dir * JumpForce; 		// Jump direction
		isMoveVer 	= dir.y == 0f; 				// Vertical move
		canGChange 	= false;

		// Start music
		if (gFirstTime && dir != Vector2.down) {
			gFirstTime = !gFirstTime;
			StartMusic.Invoke();
		}
	}

	void FixedUpdate()
	{
		if (isDead) return;
		
		ApplyGravity();
		Collision();
		Move();
		Jump();
	}

	void ApplyGravity()
	{
		Vector2 apply = rb.velocity + (dirGForce * Time.fixedDeltaTime);

		// Limit fall speed
		if 		(dirGravity == Vector2.down) 	apply.y = apply.y < -FallLimit ? -FallLimit : apply.y;
		else if (dirGravity == Vector2.up) 		apply.y = apply.y >  FallLimit ?  FallLimit : apply.y;
		else if (dirGravity == Vector2.left) 	apply.x = apply.x < -FallLimit ? -FallLimit : apply.x;
		else if (dirGravity == Vector2.right) 	apply.x = apply.x >  FallLimit ?  FallLimit : apply.x;

		rb.velocity = apply;
	}
	
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

			anim.SetTrigger("jump");
			
			rb.AddForce(jumpDir);
			isJumping = false;
		}

		void JumpCut()
		{
			if 		(dirGravity == Vector2.down 	&& rb.velocity.y < 5f) 	return;
			else if (dirGravity == Vector2.up 		&& rb.velocity.y > -5f) return;
			else if (dirGravity == Vector2.left 	&& rb.velocity.x < 5f) 	return;
			else if (dirGravity == Vector2.right 	&& rb.velocity.x > -5f) return;

			rb.AddForce(-jumpDir / 3);
		}
	#endregion

	#region (Re)Spawn
		void Respawn()
		{
			isDead 		= true;
			sr.enabled 	= false;
			rb.velocity = Vector2.zero;
			
			StartCoroutine(Transition());
		}

		IEnumerator Transition()
		{
			OnRespawnEvent.Invoke();
			yield return new WaitForSeconds(1.2f);
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
			Collider2D colGround = Physics2D.OverlapBox(checkGround.transform.position, new Vector2(transform.localScale.x, .2f), 0f, whatIsGround);

			isGrounded = colGround != null;
			
			if (isGrounded) 				canGChange = true;
			if (!wasGrounded && isGrounded) anim.SetTrigger("land");
			
			wasGrounded = isGrounded;
		}

		void OnCollisionEnter2D(Collision2D col)
		{
			switch(col.gameObject.layer)
			{
				case 10: Respawn(); return;
			}
		}
	#endregion

	public void SetCheckpoint(Transform cp)
	{
		checkPoint = cp;
		Spawn();
	}
}
