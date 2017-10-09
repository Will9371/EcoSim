using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour 
{	
	void OnTriggerEnter(Collider col)
	{
		if(MainGameManager.instance.myState == MainGameManager.PlayerState.Rabbit && col.gameObject.tag == "Animal")
			if(col.gameObject.GetComponent<AnimalController>().myAnimal == AnimalController.AnimalType.Fox)
				MainGameManager.instance.ReleaseAnimal(true);
	}
}
