/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Has all the enums for the game
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public enum DetectionState
{
    Calm, // Has no idea
    Alarmed, // Has heard things
    Aggressive, // Has seen things
    End
}

public enum AIState
{
    Sleeping,
    Idling,
    Patrolling,
    Attacking,
    Dead,
    End
}

public enum MovementType
{
    Walking,
    Running,
    Crouching,
    Jumping,
    Climbing,
    Attack,
    SpecialAttack,
    Flying,
    End
}

public enum EnemyType
{
    Alien,
    Drone,
    Facecrab,
    None,
}

public enum WeaponType
{
    LaserWeapon,
    EnemyWeapon,
}

public enum PickupType
{
    Pickup1,
}

public enum EffectType
{
    BloodSpray,
    BloodSprayLarge,
    BigBang,
    Explosion,
    Explosion2,
    Electricity,
}

public enum SoundType
{
    aliendeath,
    aliendeath2,
    aliendeath3,
    aliendeath4,
    aliendeath5,
    lasershot,
    laserreload,
    alienhiss,
    alienroar,
    aliensnarl,
    alieneating,
    alienjump,
    bulletshot,
    shield,
    chargelaser,
    congratulations,
    d,
    death,
    death2,
    deathfire,
    deng,
    ding,
    disk,
    dong,
    doorclose,
    dooropen,
    explosion,
    footstep,
    g,
    gameover,
    godmode,
    hit,
    jump,
    ladder,
    land,
    largedoor,
    laseron,
    lightningball,
    meleehit,
    meleeswing,
    menu,
    menupress,
    needshelp,
    o,
    pickup,
    press2,
    robotvoice,
    spawn,
    speech_anflugkontrolle,
    speech_brueckemitwaffen,
    speech_ichbindrin,
    speech_intro,
    speech_plan,
    speech_scheisse,
    speech_enteredstation,
    spray,
    steps,
    teleportend,
    teleportstart,
    transform,
    unable,
    victory,
}