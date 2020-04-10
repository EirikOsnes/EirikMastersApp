using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to handle the visuals of the laser pointer
/// If using laser pointers on each controller, use two LaserPointerHandlers
/// </summary>
public class LaserPointerHandler : MonoBehaviour
{
    //Point to the Laserpointer to activate
    public LaserPointer pointer;
    //The button to activate the pointer
    public OVRInput.RawButton laserPointerButton;

    // Start is called before the first frame update
    void Start()
    {
        //The laser pointer should not be active by default
        pointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.Off;
    }

    // Update is called once per frame
    void Update()
    {
        //If button is pressed, show laser, otherwise, remove it.
        if (OVRInput.Get(laserPointerButton))
        {
            pointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.On;
        } else
        {
            pointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.Off;
            pointer.cursorVisual.SetActive(false);
        }
    }
}
