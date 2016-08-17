/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Standard Singleton
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

    protected void Awake(T ptr)
    {
        if (instance != null && instance != this)
        {
            DestroyImmediate(this.gameObject, true);
            return;
        }
        else
            instance = ptr;
    }

    protected void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
