using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour 
{
	public enum TileType {Dirt, Grass, Stone};
	public TileType myTileType;
	float grassSeedTime;

	void Start () 
	{
		grassSeedTime = 5f;

		SetTileType();
		StartCoroutine(SpreadGrass());
	}

	public void SetTileType()
	{
		int i = 0;
		foreach(Transform child in transform)
		{
			if ((int)myTileType == i)
				transform.GetChild(i).gameObject.SetActive(true);
			else
				transform.GetChild(i).gameObject.SetActive(false);

			i++;
		}

		/*switch (myTileType)		//change this to a for loop later to minimize work when adding other tiletypes
		{
			case TileType.Dirt:
				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(false);
				break;
			case TileType.Grass:
				transform.GetChild(0).gameObject.SetActive(false);
				transform.GetChild(1).gameObject.SetActive(true);
				break;
		}*/
	}

	IEnumerator SpreadGrass()
	{
		Vector3 seedLoc;
		Collider[] hitColliders;
		TileController otherTile;

		while(true)
		{

			if (myTileType == TileType.Grass)
			{
				seedLoc = transform.position;
				seedLoc += new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));

				hitColliders = Physics.OverlapSphere(seedLoc, 0.1f);
				if(hitColliders.Length != 0 && hitColliders[0].gameObject.tag == "Ground")
				{
					otherTile = hitColliders[0].gameObject.GetComponent<TileController>();
					if(otherTile.myTileType == TileType.Dirt)
						otherTile.myTileType = TileType.Grass;

					otherTile.SetTileType();
				}
			}	
			yield return new WaitForSeconds (grassSeedTime);
		}
	}

}
