using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectMenuManager : MonoBehaviour 
{
	//public SteamVR_LoadLevel loadLevel;

	public List<GameObject> objectList;			//icons (list filled automatically at start)
	public int currentObject;					//current selection index

	public GameObject rabbitPool, foxPool;
	[HideInInspector] public ObjectSpawner rabbitSpawner, foxSpawner;
	[SerializeField] SeedController grassSeed;
	[SerializeField] TextMeshPro rabbitScoreText, foxScoreText;

	void Start () 
	{
		currentObject = 0;

		rabbitSpawner = rabbitPool.GetComponent<ObjectSpawner>();
		foxSpawner = foxPool.GetComponent<ObjectSpawner>();
	}

	public void MenuLeft()
	{
		objectList[currentObject].SetActive(false);
		currentObject--;

		if (currentObject < 0)
			currentObject = objectList.Count - 1;

		objectList[currentObject].SetActive(true);
	}

	public void MenuRight()
	{
		objectList[currentObject].SetActive(false);
		currentObject++;

		if (currentObject > objectList.Count - 1)
			currentObject = 0;

		objectList[currentObject].SetActive(true);
	}

	public void SpawnCurrentObject(Vector3 spawnLoc)
	{
		string newObjName = "Null";

		if(currentObject == 0)
			grassSeed.OnCreate(spawnLoc);
		else if(currentObject == 1)
			rabbitSpawner.SpawnObj(spawnLoc);
		else if(currentObject == 2)
			foxSpawner.SpawnObj(spawnLoc);
	}

	public void UpdateText()
	{
		rabbitScoreText.SetText("{0}", MainGameManager.instance.rabbitHighscore);
		foxScoreText.SetText("{0}", MainGameManager.instance.foxHighscore);
	}
}
