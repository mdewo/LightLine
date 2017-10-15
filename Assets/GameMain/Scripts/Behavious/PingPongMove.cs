using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongMove : MonoBehaviour
{
	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0, 2)]
	public float easeAmount;

	int fromWaypointIndex;
	float percentBetweenWaypoints;
	float nextMoveTime;
	Vector3 velocity = Vector3.zero;

	void Start()
	{
		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++)
		{
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}

	void Update()
	{
		if (Time.time < nextMoveTime)
		{
			velocity = Vector3.zero;
		}
		else
		{
			fromWaypointIndex %= globalWaypoints.Length;
			int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
			float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
			percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
			percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
			float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

			Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

			if (percentBetweenWaypoints >= 1)
			{
				percentBetweenWaypoints = 0;
				fromWaypointIndex++;

				if (!cyclic)
				{
					if (fromWaypointIndex >= globalWaypoints.Length - 1)
					{
						fromWaypointIndex = 0;
						System.Array.Reverse(globalWaypoints);
					}
				}
				nextMoveTime = Time.time + waitTime;
			}
			velocity = newPos - transform.position;
		}
		transform.Translate(velocity, Space.World);
	}

	float Ease(float x)
	{
		float a = easeAmount + 1;
		return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
	}

	void OnDrawGizmos()
	{
		if (localWaypoints != null)
		{
			Gizmos.color = Color.yellow;
			Vector3 prevGlobalWaypointPos = Vector3.zero;

			for (int i = 0; i < localWaypoints.Length; i++)
			{
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;

				if (i != 0)
				{
					Gizmos.DrawLine(prevGlobalWaypointPos, globalWaypointPos);
				}

				prevGlobalWaypointPos = globalWaypointPos;
			}
		}
	}
}