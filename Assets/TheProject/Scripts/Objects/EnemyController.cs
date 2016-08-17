/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Class to handle object pooling for weapons
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    EnemyType Type;

    public void SetType(EnemyType _Type)
    {
        Type = _Type;
    }

    public void OnDisable()
    {
        EnemyManager.instance.DestroyEnemy(this.gameObject, Type);
    }
}
