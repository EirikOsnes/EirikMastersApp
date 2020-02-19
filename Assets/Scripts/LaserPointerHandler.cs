using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointerHandler : MonoBehaviour
{

    public LaserPointer pointer;
    public OVRInput.RawButton laserPointerButton;

    // Start is called before the first frame update
    void Start()
    {
        pointer.laserBeamBehavior = LaserPointer.LaserBeamBehavior.Off;
    }

    // Update is called once per frame
    void Update()
    {
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
