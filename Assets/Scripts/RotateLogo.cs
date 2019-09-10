using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLogo : MonoBehaviour
{
	public Quaternion originalRotationValue; // declare this as a Quaternion
	float rotationResetSpeed = 1.0f;

	// Use this for initialization
	void Start ()
	{
		originalRotationValue = transform.rotation; // save the initial rotation
	}

	// Update is called once per frame
	void Update ()
	{
		transform.rotation = Quaternion.Slerp (transform.rotation, originalRotationValue, Time.time * rotationResetSpeed);
	}
}
