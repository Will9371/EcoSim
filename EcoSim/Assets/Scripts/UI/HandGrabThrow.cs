using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabThrow : MonoBehaviour 
{
	public SteamVR_TrackedObject trackedObj;
	private SteamVR_Controller.Device device;

	AnimalController animalController;
	[SerializeField] Transform rabbitPool, foxPool;

	public GameObject hat, nextButton;
	public bool handInHat, hatReady, onGrabNew;
	public float throwForce;
	
	GameObject animalInHand, preyInPaw;

	void Start () 
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		handInHat = false;
		hatReady = true;
		onGrabNew = false;
		animalInHand = null;
		preyInPaw = null;
	}

	void Update () 
	{
		device = SteamVR_Controller.Input((int)trackedObj.index);
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.CompareTag("Button"))
			col.gameObject.GetComponent<Button3D>().OnHandEnter(gameObject);

		else if(col.gameObject == hat)
			handInHat = true;

		if(col.gameObject.CompareTag("Animal"))
			if(col.gameObject.GetComponent<AnimalController>().myAnimal == AnimalController.AnimalType.Rabbit)
				preyInPaw = col.gameObject;
	}

	void OnTriggerExit(Collider col)
	{
		if(col.gameObject.CompareTag("Button"))
			col.gameObject.GetComponent<Button3D>().OnHandExit(gameObject);

		else if(col.gameObject == hat)
			handInHat = false;

		if(col.gameObject.CompareTag("Animal"))
			if(col.gameObject.GetComponent<AnimalController>().myAnimal == AnimalController.AnimalType.Rabbit)
				preyInPaw = null;
	}

	void OnTriggerStay(Collider col)	//fires when touching any object with a Rigidbody
	{
		if((col.gameObject.CompareTag("Animal") || col.gameObject.CompareTag("Seed")) && MainGameManager.instance.myState == MainGameManager.PlayerState.Ghost)
		{	//if touching a throwable object while in Ghost form
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
				ThrowObject(col);					//throw when releasing trigger
			else if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || onGrabNew)
			{
				GrabObject(col);					//pick up when pressing trigger
				onGrabNew = false;
			}
		}
	}

	public void GrabObject (Collider col)
	{
		if(col.gameObject.CompareTag("Animal"))
		{
			col.gameObject.GetComponent<AnimalController>().isHeld = true;
			animalInHand = col.gameObject;
		}

		col.transform.SetParent(gameObject.transform);
		col.GetComponent<Rigidbody>().isKinematic = true;	//prevent object from being knocked out of hand
		col.GetComponent<Rigidbody>().useGravity = false;

		device.TriggerHapticPulse(2000);
	}

	void ThrowObject (Collider col)	//called when player releases trigger while holding a throwable object
	{
		animalInHand = null;		

		if(handInHat)		//deactivate object
		{
			if(!hatReady)	//prevent object count from decreasing by more than 1 at a time
				return;

			//col.gameObject.transform.Translate(0, 0, -100);
			col.gameObject.SetActive(false);
			hatReady = false;
			Invoke("HatCooldown", 0.25f);
		} 
		else //throw object
		{
			Rigidbody rigidbody = col.GetComponent<Rigidbody>();
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;

			if(col.gameObject.CompareTag("Animal"))
			{
				col.gameObject.GetComponent<AnimalController>().isHeld = false;	

				animalController = col.gameObject.GetComponent<AnimalController>();
				if(animalController.myAnimal == AnimalController.AnimalType.Rabbit)
					col.transform.SetParent(rabbitPool);
				else if(animalController.myAnimal == AnimalController.AnimalType.Fox)
					col.transform.SetParent(foxPool);
			}
			else if(col.gameObject.CompareTag("Seed"))
				col.transform.SetParent(null);

			rigidbody.velocity = device.velocity * throwForce;	//set velocity based on controller movement
			rigidbody.angularVelocity = device.angularVelocity;	
		}
	}

	void HatCooldown()
	{
		hatReady = true;	
	}

	public void OnGrip()
	{
		if(animalInHand != null)
			MainGameManager.instance.PossessAnimal(animalInHand);
	}

	public bool HuntPrey()
	{
		if(preyInPaw != null)
		{
			//preyInPaw.transform.Translate(0, 0, -100);
			preyInPaw.SetActive(false);
			return true;
		}
		else
			return false;
	}
}
