using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//*Future refactor: only check for button presses in this script and call functions in other scripts
//					Other scripts (e.g. HandGrabThrow) can check for OnTriggerEnter and the like

public class ControllerInputManager : MonoBehaviour 
{
	//Hand tracking
	private SteamVR_TrackedObject trackedObj;
	public SteamVR_Controller.Device device;

	//Teleporter
	public LineRenderer laser;
	public GameObject teleportAimerObject;
	public GameObject rabbitAvatar, foxAvatar, myAvatar;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;
	public float yNudgeAmount;		//specific to teleportAimerObject height
	public bool leftController;
	private bool isLeft;

	public GameObject objectMenu;
	public ObjectMenuManager menuManager;
	public HandGrabThrow hand;
	public PlayerStats playerStats;
	private float touchLoc;

	void Start () 
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		isLeft = gameObject.GetComponent<ControllerInputManager>().leftController;
		objectMenu.SetActive(false);
		myAvatar = rabbitAvatar;
		myAvatar.SetActive(false);
		playerStats.gameObject.SetActive(false);
	}

	void Update () 
	{
		device = SteamVR_Controller.Input((int)trackedObj.index);
	
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
		{
			ButtonManager.instance.OnTriggerPress();
		}
		if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && hand.handInHat)
		{
			menuManager.SpawnCurrentObject(gameObject.transform.position);
			hand.onGrabNew = true;	//grab the object just created (normally only grab on trigger press)
		}
		else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && myAvatar.activeSelf)		//Immediately teleport to avatar location
		{																								//(before it reaches highlighted destination)
			myAvatar.GetComponent<AnimalController>().avatarDestination = myAvatar.transform.position;
			MainGameManager.instance.TeleportToAvatar(new Vector3 (myAvatar.transform.position.x, teleportLocation.y, myAvatar.transform.position.z));
			myAvatar.SetActive(false);
		}
		else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && MainGameManager.instance.myState == MainGameManager.PlayerState.Rabbit && !myAvatar.activeSelf)
		{																//if the player presses trigger while a rabbit (avatar has not been summoned)
			RaycastHit downHit;
			if(Physics.Raycast(transform.position, Vector3.down, out downHit, 4, laserMask))									//look down
				if(downHit.transform.gameObject.tag == "Ground")																//verify there is ground
				{																												
					TileController hitTile = downHit.transform.gameObject.GetComponent<TileController>();
					if(hitTile.myTileType == TileController.TileType.Grass)	//check for grass to eat
					{
						playerStats.Eat(10);
						hitTile.myTileType = TileController.TileType.Dirt;
						hitTile.SetTileType();						
					}
				}
		}		//if the player presses trigger while a fox (avatar has not been summoned)
		else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && MainGameManager.instance.myState == MainGameManager.PlayerState.Fox && !myAvatar.activeSelf)
		{
			if(hand.HuntPrey())
				playerStats.Eat(40);
		}
		else if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip) && MainGameManager.instance.myState == MainGameManager.PlayerState.Ghost)
			hand.OnGrip();		//possess animal in hand (if holding one)
		else if(device.GetPress(SteamVR_Controller.ButtonMask.Touchpad) && device.GetPress(SteamVR_Controller.ButtonMask.Grip) 
		&& MainGameManager.instance.myState != MainGameManager.PlayerState.Ghost)
		{
			myAvatar.SetActive(false);
			MainGameManager.instance.ReleaseAnimal(false);	//Deliberate release -> fade to black
		}

		if(isLeft)		//Teleport Mechanic (left hand only)
		{
			if(device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))	
			{	
				laser.gameObject.SetActive(true);
				teleportAimerObject.gameObject.SetActive(true);

				laser.SetPosition(0, gameObject.transform.position);	//set start point of laser
				RaycastHit hit, groundRay;		

								   //cast from			in direction	  storage  range   things to look for
				if(Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
				{	//check if pointing at object with a teleport collider (valid surface or barrier)
					teleportLocation = hit.point;	//record position of collision between ray and object

					if (hit.transform.gameObject.tag == "TeleportBarrier")
					{	//no movement if ray collides with a barrier
						Physics.Raycast(transform.position, Vector3.down, out groundRay, 4, laserMask);
						teleportLocation = new Vector3(player.transform.position.x, groundRay.point.y, player.transform.position.z);
					}
				}
				else
				{	//If not aimed at a valid surface, target 15 units forward in 3D space
					teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, transform.forward.y * 15 + transform.position.y, transform.forward.z * 15 + transform.position.z);

					if(Physics.Raycast(teleportLocation, Vector3.down, out groundRay, 17, laserMask))		//cast a second ray down to find a surface beneath target
						teleportLocation = new Vector3(transform.forward.x * 15 + transform.position.x, groundRay.point.y, transform.forward.z * 15 + transform.position.z);
					else 	//out of bounds condition (no floor below) -> no teleport
					{
						Physics.Raycast(transform.position, Vector3.down, out groundRay, 8, laserMask);
						teleportLocation = new Vector3(player.transform.position.x, groundRay.point.y, player.transform.position.z);
					}
				}

				laser.SetPosition(1, transform.forward * 15 + transform.position);			//set end point of laser				
				teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
			}																				
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))	//Teleport
			{
				laser.gameObject.SetActive(false);

				if(MainGameManager.instance.myState == MainGameManager.PlayerState.Ghost)
				{	//Standard teleport
					teleportAimerObject.gameObject.SetActive(false);
					player.transform.position = teleportLocation;	//*ERROR: not teleporting
				} 
				else    //Begin animal "teleport"
				{
					if(MainGameManager.instance.myState == MainGameManager.PlayerState.Rabbit)
						myAvatar = rabbitAvatar;
					if(MainGameManager.instance.myState == MainGameManager.PlayerState.Fox)
						myAvatar = foxAvatar;

					MainGameManager.instance.halo.GetComponent<RotateHalo>().SetFocus(myAvatar.transform);

					if(!myAvatar.activeSelf)	//do not reset avatar position to CameraRig if picking new destination during avatar movement
					{
						myAvatar.transform.position = new Vector3(player.transform.position.x, .2f, player.transform.position.z);
						myAvatar.SetActive(true);
					}
					myAvatar.GetComponent<AnimalController>().AvatarInit(teleportLocation);
				}
			}
		}
			
		else if (!isLeft && MainGameManager.instance.myState == MainGameManager.PlayerState.Ghost) 	//Object menu mechanic (right hand only)
		{
			if(GetComponent<ObjectMenuManager>().objectList.Count == 0)
				return;		//prevent "index out of range" errors by skipping if menu is empty

			if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
			{
				GetComponent<ObjectMenuManager>().UpdateText();
				objectMenu.SetActive(true);
			}
			else if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
				objectMenu.SetActive(false);
			
			if(objectMenu.activeSelf && device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
			{
				touchLoc = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;

				if(touchLoc < -0.2f)
					menuManager.MenuLeft();
				else if(touchLoc > 0.2f)
					menuManager.MenuRight();
			}
		}
		else if(!isLeft)	//Animal menu (when possessing an animal)
		{
			if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
				playerStats.gameObject.SetActive(true);
			else if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad))
				playerStats.gameObject.SetActive(false);
		}
	}
}
