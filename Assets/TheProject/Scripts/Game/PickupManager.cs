/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// PickupManager stores all the pickups in a pool
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class PickupManager : ObjectSpawner<PickupManager, PickupType>
{
    void Awake()
    {
        base.Awake(this);
    }

    void Start()
    {
    }

    public void Initialize()
    {
    }

    public GameObject CreatePickup(Vector3 Position, Quaternion Rotation, Vector3 Velocity, PickupType Type)
    {
        GameObject o = base.CreateObject(Position, Rotation, Type);

        if (o != null)
        {
            PickupController pc = o.GetComponent<PickupController>();

            if (pc != null)
                pc.SetType(Type);

            o.GetComponent<Rigidbody>().velocity = Velocity;
        }

        return o;
    }

    public void DestroyPickup(GameObject o, PickupType Type)
    {
        base.DestroyObject(o, Type);
    }

    public void DestroyAllPickups ()
    {
        foreach (PickupController obj in GetComponentsInChildren<PickupController>(true))
        {
            obj.OnDisable();
        }
    }
}
