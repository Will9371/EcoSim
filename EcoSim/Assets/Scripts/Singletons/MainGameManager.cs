using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour 
{
	public static MainGameManager instance;

	void Awake()
	{
		instance = this;
	}

	public enum PlayerState {Ghost, Rabbit, Fox};
	public PlayerState myState;
	Vector3 rabbitScale, foxScale;

	public GameObject rabbitPool, foxPool, halo, teleportAimerObj, playerStats;
	public Transform player, laser, teleportAimer, playerHead;
	[HideInInspector] public ObjectSpawner rabbitSpawner, foxSpawner;
	[HideInInspector] public int rabbitHighscore, foxHighscore;
	AnimalController animalControl;
	RotateHalo haloSpin;
	PlayerStats myStats;
	Coroutine metabolismCoroutine;
	bool fadeRed;

	void Start()
	{
		myState = PlayerState.Ghost;
		myStats = playerStats.GetComponent<PlayerStats>();

		haloSpin = halo.GetComponent<RotateHalo>();
		halo.SetActive(false);
		rabbitScale = new Vector3(.2f, .2f, .2f);
		foxScale = new Vector3(.3f, .3f, .3f);

		rabbitSpawner = rabbitPool.GetComponent<ObjectSpawner>();
		foxSpawner = foxPool.GetComponent<ObjectSpawner>();

		rabbitSpawner.CreatePool(200);
		foxSpawner.CreatePool(50);

		//InitSpawn();
		//ResetPrefs();
		LoadPrefs();
	}

	void InitSpawn()
	{
		rabbitSpawner.SpawnObj(new Vector3(6, .2f, 6));
		rabbitSpawner.SpawnObj(new Vector3(6, .2f, -6));
		rabbitSpawner.SpawnObj(new Vector3(-6, .2f, 6));
		rabbitSpawner.SpawnObj(new Vector3(-6, .2f, -6));
		rabbitSpawner.SpawnObj(new Vector3(1, .2f, 1));

		foxSpawner.SpawnObj(new Vector3(4, .2f, 0));
		foxSpawner.SpawnObj(new Vector3(-4, .2f, 0));
	}

	void ResetPrefs()
	{
		PlayerPrefs.SetInt("RabbitHighscore", 0);
		PlayerPrefs.SetInt("FoxHighscore", 0);
	}

	void LoadPrefs()
	{
		rabbitHighscore = PlayerPrefs.GetInt("RabbitHighscore");
		foxHighscore = PlayerPrefs.GetInt("FoxHighscore");
	}

	public void PossessAnimal(GameObject animal)	//Called by HandGrabThrow.cs -> OnTriggerStay()
	{
		animal.SetActive(false);
		halo.SetActive(true);
		haloSpin.SetFocus(playerHead);

		animalControl = animal.GetComponent<AnimalController>();
		
		if(animalControl.myAnimal == AnimalController.AnimalType.Rabbit)
		{
			animal.transform.SetParent(rabbitPool.transform);
			PossessRabbit();
		}
		if(animalControl.myAnimal == AnimalController.AnimalType.Fox)
		{
			animal.transform.SetParent(foxPool.transform);
			PossessFox();
		}

		myStats.Init();
		metabolismCoroutine = StartCoroutine(MetabolismClock());
	}

	public void ReleaseAnimal(bool killed)
	{
		UpdateHighscore();

		myState = PlayerState.Ghost;
		halo.SetActive(false);
		teleportAimerObj.SetActive(false);
		playerStats.SetActive(false);

		StopCoroutine(metabolismCoroutine);

		fadeRed = killed;
		if (fadeRed)
		{
			SteamVR_Fade.View(Color.clear, 0);
			SteamVR_Fade.View(Color.red, 1);
		}
		else
		{
			SteamVR_Fade.View(Color.clear, 0);
			SteamVR_Fade.View(Color.black, 1);
		}

		Invoke("FadeIn", 1);
	}

	void FadeIn()
	{
		if (fadeRed)
		{
			SteamVR_Fade.View(Color.red, 0);
			SteamVR_Fade.View(Color.clear, 1);
		}
		else
		{
			SteamVR_Fade.View(Color.black, 0);
			SteamVR_Fade.View(Color.clear, 1);
		}

		player.localScale = new Vector3(1f, 1f, 1f);
		teleportAimer.localScale = new Vector3(1f, .1f, 1f);
		laser.GetComponent<LineRenderer>().startWidth = .05f;	
	}

	void UpdateHighscore()
	{
		if (myState == PlayerState.Rabbit)
		{
			if(myStats.offspringCount > rabbitHighscore)
			{
				rabbitHighscore = myStats.offspringCount;
				PlayerPrefs.SetInt("RabbitHighscore", rabbitHighscore);
			}
		}
		else if(myState == PlayerState.Fox)
		{
			if(myStats.offspringCount > foxHighscore)
			{
				foxHighscore = myStats.offspringCount;
				PlayerPrefs.SetInt("FoxHighscore", foxHighscore);
			}
		}
	}

	void PossessRabbit()
	{
		myState = PlayerState.Rabbit;
		halo.transform.localScale = rabbitScale/5;
		player.localScale = rabbitScale;
		teleportAimer.localScale = new Vector3(.2f, .02f, .2f);
		laser.GetComponent<LineRenderer>().startWidth = .01f;
	}

	void PossessFox()
	{
		myState = PlayerState.Fox;
		halo.transform.localScale = foxScale/5;
		player.localScale = foxScale;	
		teleportAimer.localScale = new Vector3(.3f, .03f, .3f);	
		laser.GetComponent<LineRenderer>().startWidth = .015f;
	}

	public void TeleportToAvatar(Vector3 avatarLoc)
	{
		teleportAimer.gameObject.SetActive(false);
		player.transform.position = avatarLoc;
		haloSpin.SetFocus(playerHead);
	}

	IEnumerator MetabolismClock()
	{
		while(true)
		{	
			myStats.Metabolism(1);
			yield return new WaitForSeconds(1);
		}
	}
}
