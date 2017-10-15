using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
	public Vector3 moveDirection = Vector3.up;
	public float idleTime = 1f;
	public float moveSpeed = 6.0f;

	Vector3 targetPosition;

	MovingSpikeState state = MovingSpikeState.Idle;
	float timer = 0f;

	private void Start()
	{
		timer = 0f;
	}

	private void Update()
	{
		switch (state)
		{
			case MovingSpikeState.Idle:
				{
					timer += Time.deltaTime;
					if (timer > idleTime)
					{
						timer = 0;
						GetTargetPosition();
						state = MovingSpikeState.Moving;
					}
				}
				break;
			case MovingSpikeState.Moving:
				{
					transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
					if (transform.position == targetPosition)
					{
						moveDirection = -moveDirection;
						state = MovingSpikeState.Idle;
					}
				}
				break;
			default:
				break;
		}
	}

	void GetTargetPosition()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection);
		if (hit.collider != null && !hit.collider.isTrigger)
		{
			targetPosition = (new Vector3(hit.point.x, hit.point.y, 0f)) - moveDirection * transform.localScale.x / 2;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position + moveDirection, 0.2f);
	}

	enum MovingSpikeState
	{
		Idle, Moving
	}
}