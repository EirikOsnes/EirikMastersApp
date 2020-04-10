using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes target GameObject only active while the given button is pressed.
/// </summary>
public class ShowOnToggle : MonoBehaviour
{
    //Button to activate the object.
    public OVRInput.RawButton toggleButton;
    //Object to activate and deactivate.
    public GameObject target;
    private Logger logger;

    private void Start()
    {
        logger = GameObject.FindObjectOfType<Logger>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(toggleButton))
        {
            target.SetActive(true);
        }
        else
        {
            target.SetActive(false);
        }
    }
}
