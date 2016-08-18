using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskCycle : MonoBehaviour
{
    public GameObject highlightOffensive1;
    public GameObject highlightOffensive2;
    public GameObject highlightOffensive3;
    public GameObject highlightOffensive4;
    public GameObject highlightDefensive1;
    public GameObject highlightDefensive2;
    public GameObject highlightDefensive3;
    public GameObject highlightDefensive4;

    public GameObject maskOffensive1;
    public GameObject maskOffensive2;
    public GameObject maskOffensive3;
    public GameObject maskOffensive4;
    public GameObject maskDefensive1;
    public GameObject maskDefensive2;
    public GameObject maskDefensive3;
    public GameObject maskDefensive4;

    public void SetMaskStatus(PlayerController pc)
    {
        switch (pc.ActiveMask_Offensive)
        {
            case MaskType.Fire:
                highlightOffensive1.SetActive(true);
                highlightOffensive2.SetActive(false);
                highlightOffensive3.SetActive(false);
                highlightOffensive4.SetActive(false);
                break;
            case MaskType.Water:
                highlightOffensive1.SetActive(false);
                highlightOffensive2.SetActive(true);
                highlightOffensive3.SetActive(false);
                highlightOffensive4.SetActive(false);
                break;
            case MaskType.Earth:
                highlightOffensive1.SetActive(false);
                highlightOffensive2.SetActive(false);
                highlightOffensive3.SetActive(true);
                highlightOffensive4.SetActive(false);
                break;
            case MaskType.Lightning:
                highlightOffensive1.SetActive(false);
                highlightOffensive2.SetActive(false);
                highlightOffensive3.SetActive(false);
                highlightOffensive4.SetActive(true);
                break;
            default:
                highlightOffensive1.SetActive(false);
                highlightOffensive2.SetActive(false);
                highlightOffensive3.SetActive(false);
                highlightOffensive4.SetActive(false);
                break;
        }

        switch (pc.ActiveMask_Defensive)
        {
            case MaskType.Ice:
                highlightDefensive1.SetActive(true);
                highlightDefensive2.SetActive(false);
                highlightDefensive3.SetActive(false);
                highlightDefensive4.SetActive(false);
                break;
            case MaskType.Wind:
                highlightDefensive1.SetActive(false);
                highlightDefensive2.SetActive(true);
                highlightDefensive3.SetActive(false);
                highlightDefensive4.SetActive(false);
                break;
            case MaskType.Metal:
                highlightDefensive1.SetActive(false);
                highlightDefensive2.SetActive(false);
                highlightDefensive3.SetActive(true);
                highlightDefensive4.SetActive(false);
                break;
            case MaskType.Nature:
                highlightDefensive1.SetActive(false);
                highlightDefensive2.SetActive(false);
                highlightDefensive3.SetActive(false);
                highlightDefensive4.SetActive(true);
                break;
            default:
                highlightDefensive1.SetActive(false);
                highlightDefensive2.SetActive(false);
                highlightDefensive3.SetActive(false);
                highlightDefensive4.SetActive(false);
                break;
        }

        if (pc.OwnedMasks.ContainsKey(MaskType.Fire))
            maskOffensive1.SetActive(true);
        else
            maskOffensive1.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Water))
            maskOffensive2.SetActive(true);
        else
            maskOffensive2.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Earth))
            maskOffensive3.SetActive(true);
        else
            maskOffensive3.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Lightning))
            maskOffensive4.SetActive(true);
        else
            maskOffensive4.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Ice))
            maskDefensive1.SetActive(true);
        else
            maskDefensive1.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Wind))
            maskDefensive2.SetActive(true);
        else
            maskDefensive2.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Metal))
            maskDefensive3.SetActive(true);
        else
            maskDefensive3.SetActive(false);

        if (pc.OwnedMasks.ContainsKey(MaskType.Nature))
            maskDefensive4.SetActive(true);
        else
            maskDefensive4.SetActive(false);
    }
}
