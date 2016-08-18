/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// LevelManager has the complete level below it and would do level changes
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public class LevelManager : Singleton<LevelManager>
{
    GameState CurrentSetup = GameState.Menu;

    void Awake()
    {
        base.Awake(this);
    }

    public void Initialize()
    {
    }

    void Update()
    {
    }
}
