/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// ParticleManager stores all the particles in a pool
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class ParticleManager : ObjectSpawner<ParticleManager, EffectType>
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

    public GameObject CreateEffect(Vector3 Position, Quaternion Rotation, Vector3 Velocity, EffectType Type)
    {
        GameObject o = base.CreateObject(Position, Rotation, Type);

        if (o != null)
        {
            ParticleController pc = o.GetComponent<ParticleController>();

            if (pc != null)
                pc.SetType(Type);

            o.GetComponent<Rigidbody>().velocity = Velocity;
            pc.PlayParticleEffect(1.5f);
        }

        return o;
    }

    public void DestroyEffect(GameObject o, EffectType Type)
    {
        base.DestroyObject(o, Type);
    }
}
