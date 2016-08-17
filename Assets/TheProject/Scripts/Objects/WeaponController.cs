/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// This handles object pooling for weapons
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    WeaponType Type;
    float ShotTime;

    void Start()
    {
        ShotTime = Time.time;
    }

    void Update()
    {
        if (Time.time - ShotTime > 3f)
            this.gameObject.SetActive(false);
    }

    public void SetType(WeaponType _Type)
    {
        Type = _Type;
    }

    void OnEnable()
    {
        ShotTime = Time.time;

        var systems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem system in systems)
        {
            system.Clear();
            system.Play();
        }
    }

    public void OnDisable()
    {
        WeaponManager.instance.DestroyWeapon(this.gameObject, Type);
    }
}
