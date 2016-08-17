using UnityEngine;
using System.Collections;

public enum MaskType
{
    Fire,
    Water,
    Ice,
    Wind,
    Metal,
    Nature,
    Earth,
    Lightning
}

public class Mask
{
    public MaskType Type;
    public int Owner;
}
