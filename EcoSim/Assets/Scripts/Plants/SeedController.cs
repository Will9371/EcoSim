using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedController : MonoBehaviour 
{
	public void OnCreate(Vector3 spawnLoc)
	{
		Instantiate(gameObject, spawnLoc, Quaternion.identity);
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.CompareTag("Ground"))
		{
			TileController tile = col.gameObject.GetComponent<TileController>();

			if(tile.myTileType == TileController.TileType.Dirt)
			{
				tile.myTileType = TileController.TileType.Grass;
				tile.SetTileType();
				Destroy(gameObject, 0);
			}
		}	
	}
}
