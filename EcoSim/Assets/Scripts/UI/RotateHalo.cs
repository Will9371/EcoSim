using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHalo : MonoBehaviour 
{
	Transform focus;

	void Update()
	{
		transform.Rotate(0, 90 * Time.deltaTime, 0);
		transform.position = new Vector3(focus.position.x, 0.05f, focus.position.z);
	}

	public void SetFocus(Transform newFocus)
	{
		focus = newFocus;
	}
}
