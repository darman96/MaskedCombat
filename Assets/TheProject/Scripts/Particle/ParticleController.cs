/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Manages object pooling of particles

using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{
    public float multiplier = 1;
    float TTL;
    float SpawnTime;
    EffectType Type;

    void Awake()
    {
        TTL = 10;
    }

    public void SetType(EffectType _Type)
    {
        Type = _Type;
    }

    void OnEnable()
    {
        SpawnTime = Time.time;
        TTL = 10;
    }

    void OnDisable()
    {
        ParticleManager.instance.DestroyEffect(this.gameObject, Type);
    }

    void Update()
    {
        if (Mathf.Abs (Time.time - SpawnTime) > TTL)
            ParticleManager.instance.DestroyEffect(this.gameObject, Type);
    }

    public void PlayParticleEffect(float _TTL)
    {
        TTL = _TTL;

        var systems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem system in systems)
        {
            system.startSize *= multiplier;
            system.startSpeed *= multiplier;
            system.startLifetime *= Mathf.Lerp(multiplier, 1, 0.5f);
            system.Clear();
            system.Play();
        }
    }
}
