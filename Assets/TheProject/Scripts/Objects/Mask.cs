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

public class Mask : MonoBehaviour
{
    public MaskType Type;
    public int Owner;
}
