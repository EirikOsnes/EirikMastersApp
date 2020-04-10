using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotates the transform to face Vector3.zero.
/// </summary>
public class LookAtCenter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.LookAt(Vector3.zero);
    }

}
