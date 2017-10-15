using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
	const float deltaAngle = 45f;

	public float rotateSpeed = 25f;
	public float spawnTime = 1f;

	public Bullet bulletObj;
	public float bulletSpeed = 8.0f;
	public Transform bulletPoints;

	SpawnerState state;
	Quaternion targetRotate;
	float timer;
	bool isBullet;

	private void Start()
	{
		GetTargetRotate();
		state = SpawnerState.Rotate;
	}

	private void Update()
	{
		switch (state)
		{
			case SpawnerState.Rotate:
				{
					transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotate, rotateSpeed * Time.deltaTime);
					if (transform.rotation == targetRotate)
					{
						state = SpawnerState.Spawn;
						timer = 0f;
						isBullet = false;
					}
				}
				break;
			case SpawnerState.Spawn:
				{
					timer += Time.deltaTime;

					if (!isBullet && timer >= spawnTime / 2)
					{
						SpawnBullet();
						isBullet = true;
					}

					if (timer > spawnTime)
					{
						GetTargetRotate();
						state = SpawnerState.Rotate;
					}
				}
				break;
			default:
				break;
		}
	}

	void GetTargetRotate()
	{
		float currentAngle = transform.rotation.eulerAngles.z;
		targetRotate = Quaternion.Euler(new Vector3(0f, 0f, currentAngle + deltaAngle));
	}

	void SpawnBullet()
	{
		for (int i = 0; i < bulletPoints.childCount; i++)
		{
			Transform point = bulletPoints.GetChild(i);
			Bullet bullet = Instantiate(bulletObj, point.position, transform.rotation);
			bullet.speed = bulletSpeed;
			bullet.direction = (point.position - transform.position).normalized;
			bullet.ParentInstanceID = GetInstanceID();
		}
	}

	enum SpawnerState
	{
		Rotate,
		Spawn
	}
}