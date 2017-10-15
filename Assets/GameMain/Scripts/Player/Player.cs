using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
	[Header("Move Settings")]
	public float moveSpeed = 16f;
	float velocityXSmoothing;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;

	[Header("Jump Settings")]
	public bool doubleJump;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	bool canDoubleJump;

	[Header("Wall Sliding Settings")]
	public float wallSlideSpeedMax = 3.0f;
	public float wallStickTime = .25f;
	public Vector2 wallJumpClimb = new Vector2(10f, 20f);
	public Vector2 wallJumpOff = new Vector2(10f, 20f);
	public Vector2 wallLeap = new Vector2(10f, 20f);
	float timeToWallUnstick;
	bool wallSliding;
	int wallDirX;

	[Header("Effect")]
	public ParticleSystem playerDeadEffect;

	[Header("Tags")]
	public string damageObjTag;

	PlayerInput input;
	public PlayerInput InputValue { get { return input; } }
	Vector3 velocity;

	[Header("Player Control")]
	public bool canMove = true;
	public bool MoveControl { set; get; }

	Controller2D controller;

	public bool Grounded { get { return controller.collisions.below; } }

	private void Awake()
	{
		controller = GetComponent<Controller2D>();

		// Trail Renderer
		/*
		TrailRenderer renderer = GetComponent<TrailRenderer>();
		if (renderer)
		{
			Debug.Log("Set Trail Renderer Layer");
			renderer.sortingLayerName = "Character";
			renderer.sortingOrder = 5;
		}*/

		MoveControl = false;
	}

	void Start()
	{
		gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	private void OnEnable()
	{
		velocity = Vector3.zero;
	}

	void Update()
	{
		if (MoveControl)
		{
			MoveControl = false;
			canMove = true;
			return;
		}

		if (!canMove)
			return;

		input.Reset();
		input.horizontal = Input.GetAxisRaw("Horizontal");
		input.vertical = Input.GetAxisRaw("Vertical");

		CalculateVelocity(moveSpeed);

		HandleWallSliding();

		if (Input.GetButtonDown("Jump"))
		{
			input.jumpButtonDown = true;
			OnJumpInputDown();
		}

		if (Input.GetButtonUp("Jump"))
		{
			input.jumpButtonUp = true;
			OnJumpInputUp();
		}

		controller.Move(velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			}
			else
			{
				velocity.y = 0;
			}

			if (controller.collisions.below)
			{
				canDoubleJump = true;
			}
		}
	}

	public void OnJumpInputDown()
	{
		if (wallSliding)
		{
			if (wallDirX == input.DirX)
			{
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			}
			else if (input.DirX == 0)
			{
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			}
			else
			{
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}

		if (controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				if (input.DirX != -Mathf.Sign(controller.collisions.slopeNormal.x))
				{
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
			else
			{
				velocity.y = maxJumpVelocity;
			}
		}
		else if (doubleJump && canDoubleJump && !wallSliding)
		{
			velocity.y = maxJumpVelocity;
			canDoubleJump = false;
		}
	}

	public void OnJumpInputUp()
	{
		if (velocity.y > minJumpVelocity)
		{
			velocity.y = minJumpVelocity;
		}
	}

	void HandleWallSliding()
	{
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;

		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below /*&& velocity.y < 0*/)
		{
			wallSliding = true;
			canDoubleJump = true;

			if (velocity.y < -wallSlideSpeedMax)
			{
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0)
			{
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.DirX != wallDirX && input.DirX != 0)
				{
					timeToWallUnstick -= Time.deltaTime;
				}
				else
				{
					timeToWallUnstick = wallStickTime;
				}
			}
			else
			{
				timeToWallUnstick = wallStickTime;
			}
		}
	}

	void CalculateVelocity(float speed)
	{
		float targetVelocityX = input.horizontal * speed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
	}

	void PlayPlayerDeadEffect()
	{
		if (playerDeadEffect != null)
		{
			ParticleSystem ps = Instantiate(playerDeadEffect, transform.position, Quaternion.identity);
			ps.Play();
			Destroy(ps, ps.main.duration + 0.5f);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == damageObjTag)
		{
			Dead();
		}
	}

	void Dead()
	{
		PlayPlayerDeadEffect();
		EventManager.Invoke(GameEventType.onPlayerDead);
		gameObject.SetActive(false);
	}

	public void OnBulletHit()
	{
		Dead();
	}

	public void Rebirth(Vector3 position)
	{
		gameObject.SetActive(true);
		transform.position = position;
	}
}

public struct PlayerInput
{
	public float horizontal;
	public float vertical;
	public bool jumpButtonDown;
	public bool jumpButtonUp;

	public int DirX { get { return (int)Mathf.Sign(horizontal); } }
	public int DirY { get { return (int)Mathf.Sign(vertical); } }

	public void Reset()
	{
		horizontal = vertical = 0;
		jumpButtonDown = jumpButtonUp = false;
	}
}
