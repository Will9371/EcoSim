using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISeekFood : MonoBehaviour 
{
	/*public string critterType = "Vegetable";

	AnimalController myAnimal;
	Collider closest;

	void Start () 
	{
		myAnimal = GetComponent<AnimalController>();
	}

	public void SeekFood(List<Collider> visibleFood) 
	{
		// Find the closest edible critter to us.
		closest = null;
		float dist = Mathf.Infinity;

		foreach(Collider c in visibleFood) 
		{
			float d = Vector3.Distance(transform.position, c.transform.position);
			//find the closest critter
			if(closest == null || d < dist) 
			{
				closest = c;
				dist = d;
			}
		}

		if(closest == null) 
			return;		// No valid food targets exist.

		else 
		{
			Debug.Log("closest food found at " + closest);
			// Now we want to move towards this closest edible critter
			Vector3 dir = closest.transform.position - transform.position;
			WeightedDirection wd = new WeightedDirection( dir, 1 );
			myAnimal.desiredDirections.Add( wd );
		}
	}*/
}
