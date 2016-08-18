/*
Copyright 2016 TacDev [http://www.tacdev.eu] Contact: support@tacdev.eu
*/

/// Has all the enums for the game
/// Programmed by Ralf Mengwasser [support@tacdev.eu]

using UnityEngine;
using System.Collections;

public enum PickupType
{
    FireMask,
    WaterMask,
    IceMask,
    WindMask,
    MetalMask,
    NatureMask,
    EarthMask,
    LightningMask
}

public enum EffectType
{
    BloodSpray,
    BloodSprayLarge,
    BigBang,
    Explosion,
    Electricity,
    Lightning
}

public enum SoundType
{
    fire,
    water,
    ice,
    wind,
    metal,
    nature,
    earth,
    lightning,

    lasershot,
    bulletshot,
    congratulations,
    d,
    death,
    death2,
    deathfire,
    deng,
    ding,
    disk,
    dong,
    explosion,
    footstep,
    g,
    gameover,
    godmode,
    hit,
    jump,
    land,
    laseron,
    lightningball,
    meleehit,
    meleeswing,
    menu,
    menupress,
    o,
    pickup,
    spawn,
    spray,
    steps,
    teleportend,
    teleportstart,
    transform,
    unable,
    victory,
}

public enum WeaponType
{
    Fireball,
    Water,
    Earth,
    Lightning
}