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
    Lightning,
    NONE
}

public class Mask : MonoBehaviour
{
    public float PickupDelay = 2.0f;
    public GameObject Highlight;

    public MaskType Type;
    public bool IsOffensive;
    public int Owner;

    private float DroppedTime;

    void OnEnable()
    {
        DroppedTime = Time.time;
    }

    void Update()
    {
        if (CanBePickedUp)
            Highlight.SetActive(true);
        else
            Highlight.SetActive(false);
    }

    public bool CanBePickedUp
    {
        get
        {
            return (Time.time - DroppedTime > PickupDelay);
        }
        private set { }
    }
}
