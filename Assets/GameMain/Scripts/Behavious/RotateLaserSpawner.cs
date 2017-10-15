using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLaserSpawner : MonoBehaviour
{
	public float defaultLaserLength = 30f;
	public LineRenderer[] laserPoints;

	private void Update()
	{
		for (int i = 0; i < laserPoints.Length; i++)
		{
			Laser(i);
		}
	}

	void Laser(int index)
	{
		RaycastHit2D hit = Physics2D.Raycast(laserPoints[index].transform.position, laserPoints[index].transform.position - transform.position);
		if (hit.collider != null)
		{
			laserPoints[index].positionCount = 2;
			if (hit.collider.tag == "Player")
			{
				Player player = hit.collider.GetComponent<Player>();
				if (player != null)
					player.OnBulletHit();
			}

			laserPoints[index].SetPosition(0, laserPoints[index].transform.position);
			laserPoints[index].SetPosition(1, hit.point);
		}
		else
		{
			laserPoints[index].positionCount = 2;
			laserPoints[index].SetPosition(0, laserPoints[index].transform.position);
			laserPoints[index].SetPosition(1, laserPoints[index].transform.position + (laserPoints[index].transform.position - transform.position).normalized * defaultLaserLength);
		}
	}
}