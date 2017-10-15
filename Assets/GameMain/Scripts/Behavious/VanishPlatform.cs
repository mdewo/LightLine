using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishPlatform : MonoBehaviour
{
	public float vanishTimeInterval = 1.5f;
	public float disabledTime = 2.0f;

	float vanishTime;
	bool vanishControl;

	SpriteRenderer sr;
	BoxCollider2D boxCollider;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		vanishControl = false;
		boxCollider = GetComponent<BoxCollider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (vanishControl)
			return;

		if (collision.gameObject.tag == "Player")
		{
			ContactPoint2D[] points = collision.contacts;
			foreach (ContactPoint2D point in points)
			{
				if (point.point.y >= transform.position.y)
				{
					// Vanish
					if (!vanishControl)
						StartCoroutine(Vanish());

					break;
				}
			}
		}
	}

	IEnumerator Vanish()
	{
		vanishControl = true;
		vanishTime = 0;
		Color color;
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(0.5f);

		// Disable Component
		while (vanishTime < vanishTimeInterval)
		{
			color = sr.color;
			color.a = Mathf.Lerp(1.0f, 0, vanishTime / vanishTimeInterval);
			sr.color = color;
			vanishTime += Time.deltaTime;
			yield return null;
		}
		sr.enabled = false;
		boxCollider.enabled = false;

		// Wait a Few Seconds
		yield return new WaitForSeconds(disabledTime);

		// Try to Enable Component
		while(true)
		{
			Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, bounds.size, 0);
			bool hasPlayer = false;
			foreach (Collider2D collider in colliders)
			{
				if (collider.tag == "Player")
				{
					hasPlayer = true;
					break;
				}
			}

			if (!hasPlayer)
				break;

			yield return null;
		}

		sr.enabled = true;
		boxCollider.enabled = true;
		color = sr.color;
		color.a = 1.0f;
		sr.color = color;

		// Reset Vanish Control
		vanishControl = false;
	}
}