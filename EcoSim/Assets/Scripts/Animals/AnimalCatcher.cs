using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCatcher : MonoBehaviour 
{
	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.CompareTag("Animal"))
			col.gameObject.SetActive(false);
		if(col.gameObject.CompareTag("Seed"))
			Destroy(col.gameObject, 0);
	}
}
