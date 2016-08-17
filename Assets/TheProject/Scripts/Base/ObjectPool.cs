/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Standard object pool
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
    Queue<GameObject> Pool = new Queue<GameObject>();
    GameObject PooledObject;
    Transform Parent;
    int MaxSize=100;
    int Size=0;
    bool WorldPos = true;

    public void SetObjectPool(GameObject _PooledObject, Transform _Parent, int _MaxSize, bool _WorldPos = true)
    {
        PooledObject = _PooledObject;
        Parent = _Parent;
        MaxSize = _MaxSize;
        WorldPos = _WorldPos;
    }

    public GameObject GetObject()
    {
        if (Pool.Count > 0)
            return Pool.Dequeue();

        if (Size < MaxSize)
        {
            GameObject newobj = (GameObject)GameObject.Instantiate(PooledObject);

            newobj.transform.SetParent(Parent, WorldPos);

            Size++;

            return newobj;
        }

        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        Pool.Enqueue(obj);
    }
}
