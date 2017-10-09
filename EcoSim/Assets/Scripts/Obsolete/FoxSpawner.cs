using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxSpawner : MonoBehaviour 
{
    public static FoxSpawner current;
    public GameObject pooledObj; //the prefab of the object in the object pool
    public int objAmt = 200; //the number of objects you want in the object pool
    public List<GameObject> pooledObjs; //the object pool
    public static int objPoolNum = 0; //a number used to cycle through the pooled objects
	private bool poolEmpty;

    void Awake()
    {
        current = this; //makes it so the functions in ObjectPool can be accessed easily anywhere
    }

    void Start()	//create pool
    {
        pooledObjs = new List<GameObject>();
        for (int i = 0; i < objAmt; i++)
        {
            GameObject obj = Instantiate(pooledObj);
			obj.transform.SetParent(gameObject.transform);
            obj.SetActive(false);
            pooledObjs.Add(obj);
        }
		//SpawnObj(new Vector3(4, .2f, 0));
		//SpawnObj(new Vector3(-4, .2f, 0));
    }

    public void SpawnObj(Vector3 spawnLoc)
    {
		GameObject selectedObj = GetPooledObj();
        selectedObj.transform.position = spawnLoc;					
        selectedObj.SetActive(true);
    }

	public GameObject GetPooledObj()
	{
		poolEmpty = true;
		
	    for(objPoolNum = 0; objPoolNum < objAmt; objPoolNum++)
		{
			if (!pooledObjs[objPoolNum].activeInHierarchy)
			{
				poolEmpty = false;
				break;
			}
		}
	
		if(poolEmpty)	//if we’ve run out of objects in the pool too quickly, create a new one and add to list
	    {
	        GameObject obj = Instantiate(pooledObj);
			obj.transform.SetParent(gameObject.transform);
	        pooledObjs.Add(obj);
	        objAmt++;
	        objPoolNum = objAmt - 1;
	    }
	    return pooledObjs[objPoolNum];
	}
}
