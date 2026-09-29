using UnityEngine;
using System.Collections.Generic;

public class PoolingObject : MonoBehaviour
{
    public static PoolingObject Instance;
    Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetObject(GameObject prefab, Transform parent)
    {
        string key = prefab.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<GameObject>();
        }

        
        if (poolDictionary[key].Count > 0)
        {
            GameObject obj = poolDictionary[key].Dequeue();
            obj.transform.SetParent (parent, false);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject newObj = Instantiate(prefab, parent, false);
            newObj.name = key;
            return newObj;
        }
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent (transform);
        poolDictionary[obj.name].Enqueue(obj);
    }
}
