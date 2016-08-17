/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// WeaponManager stores all the spawned weapons.. its pooled.
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class WeaponManager : ObjectSpawner<WeaponManager, WeaponType>
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

    public GameObject CreateWeapon(Vector3 Position, Quaternion Rotation, Vector3 Velocity, WeaponType Type)
    {
        GameObject o = base.CreateObject(Position, Rotation, Type);

        if (o != null)
        {
            WeaponController pc = o.GetComponent<WeaponController>();

            if (pc != null)
                pc.SetType(Type);

            o.GetComponent<Rigidbody>().velocity = Velocity;
        }

        return o;
    }

    public void DestroyWeapon(GameObject o, WeaponType Type)
    {
        base.DestroyObject(o, Type);
    }

    public void DestroyAllWeapons()
    {
        foreach (WeaponController obj in GetComponentsInChildren<WeaponController>(true))
        {
            obj.OnDisable();
        }
    }
}
