using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//OBSOLETE: Move this code to AnimalController and/or AI scripts and delete this script
public class AnimalManager : MonoBehaviour 
{
	/*public static AnimalManager instance;

	void Awake()
	{
		instance = this;
	}

	const int RABBIT = 0;
	const int FOX = 1;

	const int FORWARD = 1;
	const int LEFT = 2;
	const int RIGHT = 3;
	const int STOP = 4;

	Animator myAnim;

	RaycastHit frontHit, downHit;
	bool isTurning;
	public int direction;

	public void Move(int myAnimal, GameObject obj)
	{
		if(myAnimal == RABBIT)
			RabbitMove(obj);
		else if(myAnimal == FOX)
			FoxMove(obj);		
	}

	void SetDirection()
	{
		isTurning = false;
		direction = Random.Range(0, 4);	
	}

	//move to SeekGrass
	void RabbitMove(GameObject obj)
	{
		if(Physics.Raycast(obj.transform.position, Vector3.down, out downHit, 1f))	//look down to decide whether to eat
		{	//verify ground is below (error prevention)
			if (downHit.transform.gameObject.tag == "Ground")
			{
				TileController tile = downHit.transform.gameObject.GetComponent<TileController>();
		
				if (tile.myTileType == TileController.TileType.Grass)
				{	//Stop and eat if there is grass below
					direction = STOP;	//will cause AnimalController to call Graze() function
					StartCoroutine(RemoveGrass(tile, 1f));
				}
				else  //if no grass below, go somewhere else
					SetDirection();

				myAnim = obj.GetComponent<Animator>();

				if (direction <= FORWARD)
					myAnim.SetTrigger("Hop");
				else if (direction == LEFT || direction == RIGHT)
					myAnim.SetTrigger("Walk");
				else if (direction == STOP)
					myAnim.SetTrigger("Graze");
			}
		}
	}

	//move to SeekMeat
	void FoxMove(GameObject obj)
	{
		SetDirection();
		//picking animation not necessary yet because only one fox animation
	}

	IEnumerator RemoveGrass(TileController tile, float delay)
	{
		yield return new WaitForSeconds(delay);
		tile.myTileType = TileController.TileType.Dirt;
		tile.SetTileType();
	}*/
}
