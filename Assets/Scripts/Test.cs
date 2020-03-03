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
    public GameObject correct;
    public float MinValue;
    public float MaxValue;
    public float Spread;
    private float startTime;
    private float observedTime = -1f;
    public float TimeUsed;
    private bool testStarted = false;
    public float AggregatedRotation;
    public float leftAngle = 360;
    public float rightAngle;
    private float leftMinAngle = 360;
    private float leftMaxAngle = 360;
    private float lastRotation;
    GameObject forwardDirection;
    public float degrees;
    public float TimeAfterObserved;
    public float distance;

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

    public void SetFOV(TestCreator.FieldOfView fov, float degrees)
    {
        FieldOfView = fov;
        this.degrees = degrees;
    }


    //Get Methods
    public float GetDegreesUsed()
    {
        return 360 - (leftAngle-rightAngle);
    }

    public bool IsCorrectChosen()
    {
        return correct == selected;
    }

    public float GetCorrectRotation()
    {
        return (correct.transform.rotation.eulerAngles.y);
    }

    public float GetSelectedRotation()
    {
        if (selected == null) return -1;
        return (selected.transform.rotation.eulerAngles.y);
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
        if (go == null) throw new NullReferenceException("Gameobject is null");
        if (go.transform == null) throw new NullReferenceException("Transform is null");
        if(TestType == TestCreator.TestType.Height)
        {
            return go.transform.lossyScale.y;
        }
        else
        {
            Renderer r = go.GetComponentInChildren<Renderer>();
            if (r == null) throw new NullReferenceException("Renderer is null");
            if (r.materials[0] == null) throw new NullReferenceException("Material is null");
            if (r.materials[0].color == null) throw new NullReferenceException("Color is null");
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
            float currentRotation = forwardDirection.transform.rotation.eulerAngles.y;
            float diff = angleDiff(lastRotation, currentRotation);
            if (diff < 0) //Head moved towards the right
            {
                if (currentRotation > rightAngle && currentRotation < leftAngle)
                {
                    rightAngle = currentRotation;
                }
            } else
            {
                if (currentRotation < leftAngle && currentRotation > rightAngle)
                {
                    leftAngle = currentRotation;
                }
            }
            
            if (Math.Abs(diff) > 0.5)
            {
                AggregatedRotation += Math.Abs(diff);
                lastRotation = currentRotation;
            }

            //Check to see if correct building is within vision.
            //Possible that the angles are inverted between headset and buildings, so must use negative.
            if(observedTime < 0)
            {
                float delta = angleDiff(currentRotation, GetCorrectRotation());
                if (Math.Abs(delta) <= degrees/2)
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
        return (observedTime == -1) ? -1 : Time.time - observedTime;
    }

    public float __GetCurrentRotation()
    {
        return forwardDirection.transform.rotation.eulerAngles.y;
    }

    public float __GetDegrees()
    {
        return degrees;
    }

    private float angleDiff(float a1, float a2)
    {
        return (a1 - a2 + 180 + 360) % 360 - 180;
    }

    public GameObject GetCorrect()
    {
        return correct;
    }
}
