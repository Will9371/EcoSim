using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyData : MonoBehaviour 
{
	public enum ObjType{Ignore, Grass, Rabbit, Fox};
	public ObjType myType;
	public GameObject myObjID;

	public MyData (GameObject id, ObjType type)
	{
		myObjID = id;
		myType = type;
	}
}
