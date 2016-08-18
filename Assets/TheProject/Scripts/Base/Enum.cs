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
    electric,
    music,
    hit1,
    hit2,
    hit3,
    hitoff1,
    hitoff2,
    hitoff3,
    invisible,
    power_pickup,
    power_up,

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
    Fire,
    Water,
    Earth,
    Lightning
}