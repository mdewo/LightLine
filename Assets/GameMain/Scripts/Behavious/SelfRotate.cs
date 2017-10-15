using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
	public Vector3 rotate;
	public Space space;
	public float smoothDampTime = 0.2f;

	Vector3 currentRotate = Vector3.zero;
	Vector3 currentVelocity = Vector3.zero;

	private void Update()
	{
		currentRotate = Vector3.SmoothDamp(currentRotate, rotate, ref currentVelocity, smoothDampTime);
		transform.Rotate(currentRotate * Time.deltaTime, space);
	}
}