/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// This handles object pooling for pickups
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour
{
    PickupType Type;

    public void SetType(PickupType _Type)
    {
        Type = _Type;
    }

    public void OnDisable()
    {
        PickupManager.instance.DestroyPickup(this.gameObject, Type);
    }
}
