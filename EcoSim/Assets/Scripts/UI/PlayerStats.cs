using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour 
{
	public TextMeshPro nameText, calorieText, fatText, offspringText;
	int myID, animalID, calorieMax, fatMax;
	[HideInInspector] public int calories, fat, offspringCount;

	public void Init()
	{
		myID = (int)MainGameManager.instance.myState;
		animalID = myID - 1; 	//enums in MainGameManager and AnimalManager are different lengths (Main includes Ghost)
		//AnimalManager.instance.GetStats(animalID);	//**

		//calorieMax = AnimalManager.instance.calorieMax;
		calories = calorieMax + 1;	//1 calorie lost immediately in coroutine
		//fatMax = AnimalManager.instance.fatMax;
		fat = 0;
		offspringCount = 0;

		if (MainGameManager.instance.myState == MainGameManager.PlayerState.Rabbit)
		{
			calorieMax = 30;
			fatMax = 20;
			nameText.SetText("Rabbit");
		}
		else if (MainGameManager.instance.myState == MainGameManager.PlayerState.Fox)
		{
			calorieMax = 50;
			fatMax = 300;
			nameText.SetText("Fox");
		}

		UpdateStats();
	}

	void UpdateStats()
	{
		calorieText.SetText("Calories: {0}/{1}", calories, calorieMax);
		fatText.SetText("Fat: {0}/{1}", fat, fatMax);
		offspringText.SetText("Offspring: {0}", offspringCount);
	}

	public void Metabolism(int calorieLoss)
	{
		calories -= calorieLoss;

		if(calories < 0)
			MainGameManager.instance.ReleaseAnimal(false); //Starvation -> fade to black

		UpdateStats();
	}

	public void Eat(int mealSize)
	{
		calories += mealSize;

		if(calories > calorieMax)	
		{
			fat += (calories - calorieMax);
			calories = calorieMax;			

			if(fat > fatMax)	
			{
				Reproduce();
				offspringCount += 1;
				fat = 0;
			}
		}

		UpdateStats();
	}

	void Reproduce()
	{
		Vector3 spawnLoc = new Vector3(transform.position.x, 0.2f, transform.position.z);

		if(MainGameManager.instance.myState == MainGameManager.PlayerState.Rabbit)
			MainGameManager.instance.rabbitSpawner.SpawnObj(spawnLoc);
		else if(MainGameManager.instance.myState == MainGameManager.PlayerState.Fox)
			MainGameManager.instance.foxSpawner.SpawnObj(spawnLoc);
	}
}
