using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class IE_ObjectPooler : MonoBehaviour
{
    public GameObject pooledObject;
    public int pooledAmount = 20;
    public bool willGrow = true;
    public HideFlags _HideFlags;

    public List<GameObject> pooledObjects;

    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.hideFlags = _HideFlags;
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] == null)
            {
                GameObject obj = (GameObject)Instantiate(pooledObject);
                obj.SetActive(false);
                obj.hideFlags = _HideFlags;
                pooledObjects[i] = obj;
                return pooledObjects[i];
            }
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.hideFlags = _HideFlags;
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

}