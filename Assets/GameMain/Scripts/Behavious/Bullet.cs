using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 8.0f;
	public Vector3 direction = Vector3.zero;
	public int ParentInstanceID { set; get; }

	private void Update()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, speed * Time.deltaTime);
		if (hit.collider != null && ParentInstanceID != hit.collider.GetInstanceID())
		{
			if (hit.collider.tag == "Player")
			{
				Player player = hit.collider.GetComponent<Player>();
				if (player != null)
					player.OnBulletHit();
			}

			Destroy(gameObject);
		}

		transform.Translate(direction * speed * Time.deltaTime, Space.World);
	}
}