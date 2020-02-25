using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string ID;
    public TestCreator.TestType TestType;
    public TestCreator.FieldOfView FieldOfView;
    private GameObject selected;
    private GameObject correct;
    public float MinValue;
    public float MaxValue;
    public float Spread;


    //Creation Methods

    public void SetCorrect(GameObject correct)
    {
        this.correct = correct;
    }

    public void generateID()
    {
        ID = TestType.ToString();
        ID += "-" + Guid.NewGuid().ToString().Remove(8);
    }

    //Run Methods

    public void setSelected(GameObject selected)
    {
        this.selected = selected;
    }
}
