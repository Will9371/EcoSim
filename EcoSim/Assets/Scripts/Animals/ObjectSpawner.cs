using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour 
{
    public GameObject pooledObj; 		//the prefab of the object in the object pool
    int objAmt; 						//the number of objects in the object pool
    public List<GameObject> pooledObjs; //the object pool
    public static int objPoolNum = 0; 	//a number used to cycle through the pooled objects
	private bool poolEmpty;

    public void CreatePool(int n)
    {
		objAmt = n;

        pooledObjs = new List<GameObject>();
        for (int i = 0; i < objAmt; i++)
        {
            GameObject obj = Instantiate(pooledObj);
			obj.transform.SetParent(gameObject.transform);
            obj.SetActive(false);
            pooledObjs.Add(obj);
        }
    }

    public void SpawnObj(Vector3 spawnLoc)
    {
		GameObject selectedObj = GetPooledObj();
        selectedObj.transform.position = spawnLoc;
		selectedObj.transform.SetParent(gameObject.transform);				
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
			Debug.Log("pool empty, instantiating...");
	        GameObject obj = Instantiate(pooledObj);
			obj.transform.SetParent(gameObject.transform);
	        pooledObjs.Add(obj);
	        objAmt++;
	        objPoolNum = objAmt - 1;
	    }
	    return pooledObjs[objPoolNum];
	}
}
