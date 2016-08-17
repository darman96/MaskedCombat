/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// EnemyManager is a standard spawner class for enemies. Pooled.
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class EnemyManager : ObjectSpawner<EnemyManager, EnemyType>
{
	void Awake ()
    {
        base.Awake(this);
	}

    void Start()
    {
    }

    public void Initialize()
    {
    }

    public GameObject CreateEnemy(Vector3 Position, Quaternion Rotation, Vector3 Velocity, EnemyType Type)
    {
        GameObject o = base.CreateObject(Position, Rotation, Type);

        if (o != null)
        {
            EnemyController pc = o.GetComponent<EnemyController>();

            if (pc != null)
            {
                pc.SetType(Type);
            }

            o.GetComponent<Rigidbody>().velocity = Velocity;
        }

        return o;
    }

    public void DestroyEnemy(GameObject o, EnemyType Type)
    {
        base.DestroyObject(o, Type);
    }

    public void DestroyAllEnemies()
    {
        foreach (EnemyController obj in GetComponentsInChildren<EnemyController>(true))
        {
            obj.OnDisable();
        }
    }
}
