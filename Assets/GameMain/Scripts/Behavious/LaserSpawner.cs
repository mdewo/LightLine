using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
	public float idleTime = 1.5f;
	public float laserTime = 0.8f;
	public float defaultLaserLength = 10f;
	public Transform laserPoint;

	public LaserSpawnerState state = LaserSpawnerState.Idle;
	float timer;

	LineRenderer lr;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
		lr.sortingLayerName = GetComponent<SpriteRenderer>().sortingLayerName;
	}

	private void Start()
	{
		timer = 0;
	}

	private void Update()
	{
		switch (state)
		{
			case LaserSpawnerState.AlwaysLaser:
				{
					Laser();
				}
				break;
			case LaserSpawnerState.Idle:
				timer += Time.deltaTime;
				if (timer > idleTime)
				{
					state = LaserSpawnerState.Laser;
					timer = 0;
				}
				break;
			case LaserSpawnerState.Laser:
				{
					Laser();
					timer += Time.deltaTime;
					if (timer > laserTime)
					{
						StopLaser();
						state = LaserSpawnerState.Idle;
						timer = 0;
					}
				}
				break;
			default:
				break;
		}
	}

	void Laser()
	{
		RaycastHit2D hit = Physics2D.Raycast(laserPoint.position, transform.position - laserPoint.position);
		if (hit.collider != null)
		{
			lr.positionCount = 2;
			if (hit.collider.tag == "Player")
			{
				Player player = hit.collider.GetComponent<Player>();
				if (player != null)
					player.OnBulletHit();
			}

			lr.SetPosition(0, laserPoint.position);
			lr.SetPosition(1, hit.point);
		}
		else
		{
			lr.positionCount = 2;
			lr.SetPosition(0, laserPoint.position);
			lr.SetPosition(1, laserPoint.position + (transform.position - laserPoint.position).normalized * defaultLaserLength);
		}
	}

	void StopLaser()
	{
		lr.positionCount = 0;
	}

	public enum LaserSpawnerState
	{
		Idle, Laser, AlwaysLaser
	}
}