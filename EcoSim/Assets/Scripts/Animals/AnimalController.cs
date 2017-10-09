using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour 
{
	[HideInInspector] public List<BoxCollider> grassInSight, rabbitsInSight, foxesInSight;

	public enum AnimalType {Rabbit, Fox};	//may become redundant
	public AnimalType myAnimal;
	public bool isAvatar, avatarMoving;			//isAvatar set in inspector (permanent), avatarMoving activated while player is in animal form and teleporting
	[SerializeField] PlayerStats playerStats;
	Animator myAnim;

	int myMove, forwardSeries;
	const int FORWARD = 3;
	const int STOP = 4;
	const int TURN = 5;
	float angleChange, moveTime;

	public bool isHeld;

	Transform myLoc;
	Vector3 startAngle, curAngle, vectorToTarget, cross;	//*some variables here may be obsolete
	Quaternion fromAngle, toAngle;
	float newAngle, xRot, zRot, tempYRot;
	[SerializeField] float moveSpeed, turnSpeed;
	bool isTurning, isEscaping;
	public Vector3 avatarDestination;

	RaycastHit frontHit, downHit;
	[SerializeField] GameObject fieldOfView;
	BoxCollider closest;

	public int calorieCount, fatCount;
	[SerializeField] int calorieMax, fatMax;
	int mealSize;

	LineRenderer laserEyes;	//For testing purposes, to verify animals are finding the closest object of interest

	void Start()
	{
		laserEyes = GetComponent<LineRenderer>();
		laserEyes.enabled = false;
	}

	public void OnEnable()
	{
		myLoc = gameObject.transform;
		isTurning = false;
		isEscaping = false;
		isHeld = false;
		avatarMoving = false;

		calorieCount = calorieMax;
		fatCount = 0;

		if(!isAvatar)
		{
			StartCoroutine(PickAction());
			StartCoroutine(MetabolismClock());
		}
	}

	void LookAbout()
	{
		grassInSight.Clear();
		rabbitsInSight.Clear();
		foxesInSight.Clear();

		fieldOfView.SetActive(true);
		Invoke("StopLooking", .1f);
	}

	void StopLooking()
	{
		fieldOfView.SetActive(false);
	}

	void OnTriggerEnter(Collider col)
	{
		BoxCollider boxCol = col.gameObject.GetComponent<BoxCollider>();
	
		//determine type of object that has entered my field of view and, if I care about it, add it to the appropriate list (if it is not there already)
		if(col.gameObject.tag == "Animal")
		{
			if(myAnimal == AnimalType.Fox && !rabbitsInSight.Contains(boxCol) && col.gameObject.GetComponent<AnimalController>().myAnimal == AnimalType.Rabbit)
				rabbitsInSight.Add(boxCol);
			else if(myAnimal == AnimalType.Rabbit && !foxesInSight.Contains(boxCol) && col.gameObject.GetComponent<AnimalController>().myAnimal == AnimalType.Fox)
				foxesInSight.Add(boxCol);
		}
		else if(myAnimal == AnimalType.Rabbit && col.gameObject.tag == "Ground")	//*This line somehow interferes with animal collisions (and accumulates too many)
			if(!grassInSight.Contains(boxCol) && col.gameObject.GetComponent<TileController>().myTileType == TileController.TileType.Grass)
				grassInSight.Add(boxCol);
	}
	
	IEnumerator PickAction()	//used to decide next action
	{
		while(true)
		{
			yield return new WaitForSeconds(moveTime);

			if(!isHeld && forwardSeries > 0)
			{	//If precommited to going forward, go forward
				myMove = FORWARD;
				moveTime = 1f;
				forwardSeries--;

				StandUpStraight();
				Move();
				LookAbout();
			}
			else if(!isHeld)
			{
				if (myAnimal == AnimalType.Rabbit)
				{
					closest = FindClosest(foxesInSight);

					if (closest != null)
					{
						TurnAndMove(3);
						isEscaping = true;
					}
					else
					{
						closest = FindClosest(grassInSight);	//search for grass

						if(closest != null)
						{
							//Debug.Log("animal at: " + transform.position + "closest grass at: " + closest.transform.TransformPoint(Vector3.zero));
							laserEyes.enabled = true;
							laserEyes.SetPosition(1, new Vector3(closest.transform.TransformPoint(Vector3.zero).x, 
								closest.transform.TransformPoint(Vector3.zero).y + 0.5f, closest.transform.TransformPoint(Vector3.zero).z));
						}
					}
				}
				else if (myAnimal == AnimalType.Fox)
					closest = FindClosest(rabbitsInSight);	//search for rabbits

				if(!isEscaping)
				{
					if(closest != null)	//if found predator or food
						SetTarget();	//angleChange set by SetTarget or TurnAndMove, used as target
					else   				
						SetRandomDirection();
				}
					
				StandUpStraight();
				Move();
				LookAbout();		

				if(myMove == STOP && !isEscaping) 
					Eat(10);		//*Get this value from grass tile (and generalize to fox)
			}

			//yield return new WaitForSeconds(moveTime);
		}
	}

	BoxCollider FindClosest(List<BoxCollider> visibleCols) 
	{
		closest = null;
		float dist = Mathf.Infinity;

		foreach(BoxCollider c in visibleCols) 
		{
			float d = Vector3.Distance(transform.position, c.transform.TransformPoint(Vector3.zero));	

			if(closest == null || d < dist) 
			{
				closest = c;
				dist = d;
			}
		}
		return closest;
	}

	void SetTarget()
	{
		angleChange = Vector3.Angle(transform.position, closest.transform.position);	//returns absolute value
		cross = Vector3.Cross(closest.transform.position, transform.position);	//used to get negative angles

		myMove = TURN;

		if(cross.y < 0)
			angleChange = -angleChange;

		//after setting a new heading, precommit to forward motion on next movement after turning = unit distance from self to food source
		forwardSeries = Mathf.CeilToInt(Mathf.Abs(Vector3.Distance(transform.position, closest.transform.position)));		
	}

	void TurnAndMove(int forwardSeries)
	{
		int t = Random.Range(-1, 1);
		angleChange = t * 180;
		myMove = TURN;
		forwardSeries = 3;
	}

	public void Move()
	{
		//movement code unique to specific animals
		if(myAnimal == AnimalType.Rabbit)	
			RabbitMove();	
	}

	void SetRandomDirection()
	{
		if(Random.Range(0, 2) < 1)
		{
			myMove = FORWARD;
			moveTime = 1f;
		}

		SetLeftOrRight();
	}

	void SetLeftOrRight()
	{
		float d = Random.Range(-1, 1);
		if(d < 0)
			angleChange = -90;
		else
			angleChange = 90;

		isTurning = false;
		myMove = TURN;
	}

	void RabbitMove()	//At this point, have already set the angle of closest food source or set a random direction if no food discovered
	{
		if(forwardSeries > 0 && Physics.Raycast(transform.position, Vector3.down, out downHit, 1f))	//look down to decide whether to eat
		{	//verify ground is below (error prevention)
			if (downHit.transform.gameObject.tag == "Ground")
			{
				TileController tile = downHit.transform.gameObject.GetComponent<TileController>();
		
				if (tile.myTileType == TileController.TileType.Grass)
				{	//Stop and eat if there is grass below
					myMove = STOP;	//will cause AnimalController to call Graze() function
					moveTime = 1f;
					StartCoroutine(RemoveGrass(tile, 1f));
				}
			}
		}

		myAnim = GetComponent<Animator>();

		if (myMove <= FORWARD)		//occurs randomly and when forwardSeries > 0
			myAnim.SetTrigger("Hop");
		else if (myMove == TURN)	//occurs randomly and when 
			myAnim.SetTrigger("Walk");
		else if (myMove == STOP)
			myAnim.SetTrigger("Graze");
	}

	void FoxMove() {}	//future use when fox has unique behaviors

	void Update()	//for actions involving continuous movement (e.g. lerp)
	{
		if(isHeld || myMove == STOP)
			return;

		if(isAvatar && !avatarMoving)
			return;

		laserEyes.SetPosition(0, transform.position);

		if(isAvatar && avatarMoving)
		{
			transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

			vectorToTarget = transform.position - avatarDestination;
			vectorToTarget.y = 0;

			if(vectorToTarget.magnitude <= .2f)	//on reaching target
			{
				StopCoroutine(AvatarCalibrate());
				gameObject.SetActive(false);
				MainGameManager.instance.TeleportToAvatar(avatarDestination); 
				avatarMoving = false;
			}
		}

		else if(myMove <= FORWARD)	
			myLoc.position = Vector3.Lerp(myLoc.position, myLoc.position + myLoc.forward, Time.deltaTime * moveSpeed);
		else if(myMove == TURN && !isTurning)
		{	
			isTurning = true;
			moveTime = Mathf.Abs(angleChange/turnSpeed);
			StartCoroutine(RotateMe(Vector3.up * angleChange, moveTime));		
		}
	}

	IEnumerator RotateMe(Vector3 byAngles, float moveTime)
	{
		fromAngle = transform.rotation;	
		toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);

		//Debug.Log("current angle: " + fromAngle + " changing angle by: " + byAngles + " to angle: " + toAngle + " move time: " + moveTime + " move speed: " + moveSpeed);

		for (float t = 0f; t < 1f; t += Time.deltaTime/moveTime)
		{
			transform.rotation = Quaternion.Lerp(fromAngle, toAngle, t);
			yield return null;
		}
		myMove = FORWARD;
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ground" && col.gameObject.GetComponent<TileController>().myTileType == TileController.TileType.Stone)
			TurnAndMove(1);

		if(col.gameObject.tag != "Animal")
			return;

		AnimalController otherAnimal = col.gameObject.GetComponent<AnimalController>();

		//(enum)relate = CheckRelation((int)myAnimal, (int)col.[...]myAnimal)	//*Generalize when I add another animal type
		//if (relate == myRelation.Kill)
			//Kill();
		if(myAnimal == AnimalType.Fox && otherAnimal.myAnimal == AnimalType.Rabbit && !isAvatar)	
		{
			col.gameObject.SetActive(false);
			Eat(40);	//*get this value from collider object's stats
		}

		if(isAvatar)
		{
			if(myAnimal == AnimalType.Rabbit && otherAnimal.myAnimal == AnimalType.Fox)
				MainGameManager.instance.ReleaseAnimal(true);	//fade red because killed
		}
	}

	void StandUpStraight()
	{
		xRot = transform.rotation.eulerAngles.x;	//check my current angle to see if I have fallen over
		zRot = transform.rotation.eulerAngles.z;
	
		if(transform.position.y < 0.3f && (Mathf.Abs(xRot) > 75 || Mathf.Abs(zRot) > 75))	
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); //if on ground but not standing up, then flip upright
	}

	void Eat(int mealSize)
	{
		calorieCount += mealSize;		
		if(calorieCount > calorieMax)	
		{
			fatCount += (calorieCount - calorieMax);
			calorieCount = calorieMax;			

			if(fatCount > fatMax)	
			{
				Reproduce();
				fatCount = 0;
			}
		}
	}

	void Reproduce()
	{
		if(myAnimal == AnimalType.Rabbit)
			MainGameManager.instance.rabbitSpawner.SpawnObj(transform.position - Vector3.forward);
		else if(myAnimal == AnimalType.Fox)
			MainGameManager.instance.foxSpawner.SpawnObj(transform.position - Vector3.forward * 2);
	}

	public void AvatarInit(Vector3 teleportLocation)
	{
		avatarMoving = true;		
		avatarDestination = teleportLocation;
		StartCoroutine(AvatarCalibrate());
	}

	IEnumerator MetabolismClock()
	{
		while(true)
		{
			if(!isAvatar)
			{
				calorieCount--;
				if(calorieCount < 0)
					gameObject.SetActive(false);
			}
			yield return new WaitForSeconds(1f);	//TBD: replace with starveRate, unique to animal
		}
	}

	IEnumerator AvatarCalibrate()
	{
		while(true)
		{
			transform.LookAt(new Vector3(avatarDestination.x, transform.position.y, avatarDestination.z));
			yield return new WaitForSeconds(0.5f);
		}
	}

	IEnumerator RemoveGrass(TileController tile, float delay)
	{
		yield return new WaitForSeconds(delay);
		tile.myTileType = TileController.TileType.Dirt;
		tile.SetTileType();
	}
}
