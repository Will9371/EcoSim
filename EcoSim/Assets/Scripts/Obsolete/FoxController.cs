using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxController : MonoBehaviour 
{
	public float moveSpeed, turnSpeed;
	public int calorieCount, fatCount;

	public void SetStats()
	{
		moveSpeed = 1f;
		turnSpeed = 90f;
		calorieCount = 50;
		fatCount = 0;
	}
}
