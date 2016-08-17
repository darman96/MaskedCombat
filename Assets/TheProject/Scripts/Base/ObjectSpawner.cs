/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Standard object spawner
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// T is enum
public class ObjectSpawner<M,T> : Singleton<M> where M : MonoBehaviour
{
    public int MaxPoolSizePerObject = 100;

    private Dictionary<T, ObjectPool> Cache = new Dictionary<T, ObjectPool>();

    // Will load every object name in the enum from Resources/
    // And stores it in ObjectPools that holds objects to spawn
    protected new void Awake(M ptr)
    {
        base.Awake(ptr);

        Cache.Clear();
        GameObject obj = null;

        foreach (T st in ((T[])Enum.GetValues(typeof(T))))
        {
            obj = (GameObject)Resources.Load(st.GetType().ToString() + "/" + st.ToString());

            ObjectPool pool = new ObjectPool();
            pool.SetObjectPool(obj, transform, MaxPoolSizePerObject);

            Cache.Add(st, pool);
        }
    }

    protected GameObject CreateObject(Vector3 Position, Quaternion Rotation, T Type)
    {
        GameObject o = Cache[Type].GetObject();

        if (o == null)
            return null;

        o.transform.rotation = Rotation;
        o.transform.position = Position;
        o.SetActive(true);

        return o;
    }
    protected void DestroyObject(GameObject o, T Type)
    {
        o.SetActive(false);
        Cache[Type].ReturnObject(o);
    }
}
