using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class holding live data of a Test.
/// </summary>
public class Test : MonoBehaviour
{
    public string ID;
    public TestCreator.TestType TestType;
    public TestCreator.FieldOfView FieldOfView;
    private GameObject selected;
    public GameObject correct;
    public float MinValue;
    public float MaxValue;
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
    public int Quadrant;
    public int testSet;
    public int numberOfBuildings;
    bool tutorial = false;

    //----------Creation Methods----------

    /// <summary>
    /// Set the correct answer.
    /// </summary>
    /// <param name="correct">The correct answer.</param>
    public void SetCorrect(GameObject correct)
    {
        this.correct = correct;
    }

    /// <summary>
    /// Generate a unique ID for the test.
    /// </summary>
    public void generateID()
    {
        ID = TestType.ToString();
        ID += " - Min:" + MinValue + " - Max:" + MaxValue;
        ID += " - " + FieldOfView.ToString();
    }

    /// <summary>
    /// Set the Field of View mode for the test.
    /// </summary>
    /// <param name="fov">Field of view mode.</param>
    /// <param name="degrees">Degrees in narrow field of view.</param>
    public void SetFOV(TestCreator.FieldOfView fov, float degrees)
    {
        FieldOfView = fov;
        this.degrees = degrees;
    }


    //----------Get Methods----------

    /// <summary>
    /// Return the total degrees observed during the test.
    /// </summary>
    /// <returns>Observed field in degrees.</returns>
    public float GetDegreesUsed()
    {
        return 360 - (leftAngle-rightAngle);
    }

    /// <summary>
    /// Check to see if the correct building is chosen.
    /// </summary>
    /// <returns>True if correct is chosen, false otherwise.</returns>
    public bool IsCorrectChosen()
    {
        return correct == selected;
    }

    /// <summary>
    /// Get the rotation of the correct answer.
    /// </summary>
    /// <returns>Rotation fo the correct answer in degrees.</returns>
    public float GetCorrectRotation()
    {
        return (correct.transform.rotation.eulerAngles.y);
    }

    /// <summary>
    /// Get the rotation of the selected answer.
    /// </summary>
    /// <returns>Rotation of the selected answer, -1 if none is selected.</returns>
    public float GetSelectedRotation()
    {
        if (selected == null) return -1;
        return (selected.transform.rotation.eulerAngles.y);
    }

    /// <summary>
    /// Get the determining value of the correct answer.
    /// </summary>
    /// <returns>Returns the determining value of the correct answer.</returns>
    public float GetCorrectValue()
    {
        return getValueFromBuilding(correct);
    }

    /// <summary>
    /// Get the determining value of the selected answer.
    /// </summary>
    /// <returns>Returns the determining value of the selected answer, -1 if none is selected.</returns>
    public float GetSelectedValue()
    {
        if (selected == null) return -1;
        return getValueFromBuilding(selected);
    }

    /// <summary>
    /// Gets the current time used.
    /// </summary>
    /// <returns>Time currently used.</returns>
    public float GetCurrentTime()
    {
        return Time.time - startTime;
    }

    /// <summary>
    /// Gets the current time used since the correct answer was observed.
    /// </summary>
    /// <returns>Time currently used since correct was observed, -1 if not yet observed.</returns>
    public float GetCurrentTimeSinceObserved()
    {
        return (observedTime == -1) ? -1 : Time.time - observedTime;
    }

    /// <summary>
    /// Gets the head's current rotation.
    /// </summary>
    /// <returns>Head's current rotation in degrees.</returns>
    public float __GetCurrentRotation()
    {
        return forwardDirection.transform.rotation.eulerAngles.y;
    }

    /// <summary>
    /// Gets the degrees used for this test.
    /// </summary>
    /// <returns>Degrees used for this test.</returns>
    public float __GetDegrees()
    {
        return degrees;
    }

    /// <summary>
    /// Gets the correct answer.
    /// </summary>
    /// <returns>GameObject of correct answer.</returns>
    public GameObject GetCorrect()
    {
        return correct;
    }

    /// <summary>
    /// Get the determining value of the given building. Throws errors if value can not be calculated.
    /// </summary>
    /// <param name="go">The building GameObject to find value for.</param>
    /// <returns>The determining value of the given Building.</returns>
    private float getValueFromBuilding(GameObject go)
    {
        if (go == null) throw new NullReferenceException("Gameobject is null");
        if (go.transform == null) throw new NullReferenceException("Transform is null");
        if (TestType == TestCreator.TestType.Height)
        {
            return go.transform.lossyScale.y;
        }
        else if (TestType == TestCreator.TestType.Colour)
        {
            Renderer r = go.GetComponentInChildren<Renderer>();
            if (r == null) throw new NullReferenceException("Renderer is null");
            if (r.materials[0] == null) throw new NullReferenceException("Material is null");
            if (r.materials[0].color == null) throw new NullReferenceException("Color is null");
            return r.materials[0].color.r * 255;
        }
        else if (TestType == TestCreator.TestType.Distance)
        {
            return Vector3.Distance(go.transform.position, Vector3.zero);
        }
        else return -1;
    }

    //----------Run Methods----------

    /// <summary>
    /// Runs every frame.
    /// </summary>
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
            } else //Head moved towards the left
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

    /// <summary>
    /// Starts the Test.
    /// </summary>
    public void StartTest()
    {
        forwardDirection = GameObject.Find("ForwardDirection");
        StartTimer();
        testStarted = true;
    }

    /// <summary>
    /// Select a Building.
    /// </summary>
    /// <param name="go">The Building selected.</param>
    public void SetSelected(GameObject go)
    {
        selected = go;
    }

    /// <summary>
    /// Starts the timer for the test.
    /// </summary>
    public void StartTimer()
    {
        startTime = Time.time;
    }

    /// <summary>
    /// Ends the timer for the test.
    /// </summary>
    public void EndTimer()
    {
        TimeUsed = Time.time - startTime;
        TimeAfterObserved = GetCurrentTimeSinceObserved();
    }


    /// <summary>
    /// Calculates the difference between two angles.
    /// </summary>
    /// <param name="a1">Angle 1.</param>
    /// <param name="a2">Angle 2.</param>
    /// <returns>Difference between a1 and a2.</returns>
    private float angleDiff(float a1, float a2)
    {
        return (a1 - a2 + 180 + 360) % 360 - 180;
    }
}
