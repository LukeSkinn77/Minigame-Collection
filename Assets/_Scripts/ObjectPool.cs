using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class ObjectPoolItem
    {
        public string poolName;
        public int poolID;
        public int poolAmount;
        public GameObject poolObject;
    }

    [System.Serializable]
    public class ItemObject
    {
        public int id;
        public GameObject obj;

        public ItemObject(int _id, GameObject _obj)
        {
            id = _id;
            obj = _obj;
        }
    }

    public List<ObjectPoolItem> objectPoolList;
    public List<ItemObject> objectTotalList;

    private void Awake()
    {
        Instance = this;

        objectTotalList = new List<ItemObject>();
        foreach (ObjectPoolItem poolItem in objectPoolList)
        {
            for (int i = 0; i < poolItem.poolAmount; i++)
            {
                var bullt = Instantiate(poolItem.poolObject);
                bullt.name = poolItem.poolName;
                bullt.SetActive(false);
                ItemObject item = new ItemObject(poolItem.poolID, bullt);
                objectTotalList.Add(item);
            }
        }
    }

    public GameObject GetPooledObjects(int id)
    {
        for (int i = 0; i < objectTotalList.Count; i++)
        {
            if (!objectTotalList[i].obj.activeInHierarchy && objectTotalList[i].id == id)
            {
                return objectTotalList[i].obj;
            }
        }
        for (int i = 0; i < objectPoolList.Count; i++)
        {
            if (objectPoolList[i].poolID == id)
            {
                GameObject obj = Instantiate(objectPoolList[i].poolObject);
                obj.name = objectPoolList[i].poolName;
                obj.SetActive(false);
                ItemObject item = new ItemObject(objectPoolList[i].poolID, obj);
                objectTotalList.Add(item);
                return obj;
            }
        }
        return null;
    }
}
