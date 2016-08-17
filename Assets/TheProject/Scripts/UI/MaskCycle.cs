using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MaskCycle : MonoBehaviour {

    public MaskType ActiveOffensive
    {
        get { return activeOffensive; }
        set
        {
            activeOffensive = value;
            UpdateHighlight();
        }
    }
    public MaskType ActiveDefensive
    {
        get { return activeDefensive; }
        set
        {
            activeDefensive = value;
            UpdateHighlight();
        }
    }

    public Image highlightOffensive1;
    public Image highlightOffensive2;
    public Image highlightOffensive3;
    public Image highlightOffensive4;

    public Image highlightDefensive1;
    public Image highlightDefensive2;
    public Image highlightDefensive3;
    public Image highlightDefensive4;

    private MaskType activeOffensive;
    private MaskType activeDefensive;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void UpdateHighlight()
    {
        switch(activeOffensive)
        {
            case MaskType.Fire:
                highlightOffensive1.enabled = true;
                highlightOffensive2.enabled = false;
                highlightOffensive3.enabled = false;
                highlightOffensive4.enabled = false;
                break;
            case MaskType.Water:
                highlightOffensive1.enabled = false;
                highlightOffensive2.enabled = true;
                highlightOffensive3.enabled = false;
                highlightOffensive4.enabled = false;
                break;
            case MaskType.Earth:
                highlightOffensive1.enabled = false;
                highlightOffensive2.enabled = false;
                highlightOffensive3.enabled = true;
                highlightOffensive4.enabled = false;
                break;
            case MaskType.Lightning:
                highlightOffensive1.enabled = false;
                highlightOffensive2.enabled = false;
                highlightOffensive3.enabled = false;
                highlightOffensive4.enabled = true;
                break;
        }

        switch (activeDefensive)
        {
            case MaskType.Ice:
                highlightDefensive1.enabled = true;
                highlightDefensive2.enabled = false;
                highlightDefensive3.enabled = false;
                highlightDefensive4.enabled = false;
                break;
            case MaskType.Wind:
                highlightDefensive1.enabled = false;
                highlightDefensive2.enabled = true;
                highlightDefensive3.enabled = false;
                highlightDefensive4.enabled = false;
                break;
            case MaskType.Metal:
                highlightDefensive1.enabled = false;
                highlightDefensive2.enabled = false;
                highlightDefensive3.enabled = true;
                highlightDefensive4.enabled = false;
                break;
            case MaskType.Nature:
                highlightDefensive1.enabled = false;
                highlightDefensive2.enabled = false;
                highlightDefensive3.enabled = false;
                highlightDefensive4.enabled = true;
                break;
        }
    }

}
