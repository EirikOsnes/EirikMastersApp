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
    private float startTime;
    private float observedTime = -1f;
    public float TimeUsed;
    private bool testStarted = false;
    public float AggregatedRotation;
    private float minAngle;
    private float maxAngle;
    private float lastRotation;
    GameObject forwardDirection;
    private float degrees;
    public float TimeAfterObserved;

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

    public void setFOV(TestCreator.FieldOfView fov, float degrees)
    {
        FieldOfView = fov;
        this.degrees = degrees;
    }


    //Get Methods
    public float getDegreesUsed()
    {
        return Math.Abs(maxAngle - minAngle);
    }

    public bool IsCorrectChosen()
    {
        return correct == selected;
    }

    public float GetCorrectRotation()
    {
        return correct.transform.rotation.y;
    }

    public float GetSelectedRotation()
    {
        if (selected == null) return -1;
        return selected.transform.rotation.y;
    }

    public float GetCorrectValue()
    {
        return getValueFromBuilding(correct);
    }

    public float GetSelectedValue()
    {
        if (selected == null) return -1;
        return getValueFromBuilding(selected);
    }

    private float getValueFromBuilding(GameObject go)
    {
        if(TestType == TestCreator.TestType.Height)
        {
            return go.transform.lossyScale.y;
        }
        else
        {
            Renderer r = go.GetComponentInChildren<Renderer>();
            return r.materials[0].color.r * 255;
        }
    }

    //Run Methods
    void Update()
    {
        if (testStarted)
        {
            //The running checks for this Test should only run when the Test has started.

            //Track changes in head rotation.
            float currentRotation = forwardDirection.transform.rotation.y;
            minAngle = Math.Min(minAngle, currentRotation);
            maxAngle = Math.Max(maxAngle, currentRotation);
            AggregatedRotation += Math.Abs(currentRotation - lastRotation);
            lastRotation = currentRotation;

            //Check to see if correct building is within vision.
            //Possible that the angles are inverted between headset and buildings, so must use negative.
            if(observedTime == -1f)
            {
                if(currentRotation + degrees/2 > correct.transform.rotation.y 
                    && currentRotation - degrees / 2 < correct.transform.rotation.y)
                {
                    observedTime = Time.time;
                }
            }
        }
    }

    public void StartTest()
    {
        forwardDirection = GameObject.Find("ForwardDirection");
        StartTimer();
        testStarted = true;
    }

    public void SetSelected(GameObject go)
    {
        selected = go;
    }

    public void StartTimer()
    {
        startTime = Time.time;
    }

    public void EndTimer()
    {
        TimeUsed = Time.time - startTime;
        TimeAfterObserved = Time.time - observedTime;
    }

    public float GetCurrentTime()
    {
        return Time.time - startTime;
    }

    public float GetCurrentTimeSinceObserved()
    {
        return Math.Max(0, Time.time - observedTime);
    }
}
