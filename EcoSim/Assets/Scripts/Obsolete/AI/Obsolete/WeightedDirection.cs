using System.Collections;
using UnityEngine;

public class WeightedDirection : MonoBehaviour 
{
	public readonly Vector3 direction;
	public readonly float weight;

	public WeightedDirection(Vector2 dir, float wgt) 
	{
		direction = dir.normalized;
		weight = wgt;
	}

	public enum BlendingType { BLEND, EXCLUSIVE, FALLBACK };	// UNUSED
	public BlendingType blending = BlendingType.BLEND;	

	public float speed = -1f; 									// UNUSED
}
