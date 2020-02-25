using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class has only purpose: To serve as a container for all the data that is to be logged for statistics.
/// </summary>
public class TestData
{

    private Test currentTest;
    public string TestID;
    public float TimeUsed;
    public float AggregatedRotation; //Sum of all head rotation done
    public float DegreesUsed; //Total of the 360 degrees viewed
    public TestCreator.TestType TestType;
    public TestCreator.FieldOfView FieldOfView;
    public bool CorrectChosen;
    public float CorrectValue;
    public float ChosenValue;
    public float CorrectAngle;
    public float ChosenAngle;
    public float TimeAfterObserved;
    public float MinValue;
    public float MaxValue;
    public float Spread;
    

    public TestData (Test test)
    {
        currentTest = test;
        pollData();
    }

    private void pollData()
    {
        TestID = currentTest.ID;
        TimeUsed = currentTest.TimeUsed;
        AggregatedRotation = currentTest.AggregatedRotation;
        DegreesUsed = currentTest.getDegreesUsed();
        TestType = currentTest.TestType;
        FieldOfView = currentTest.FieldOfView;
        CorrectChosen = currentTest.IsCorrectChosen();
        CorrectValue = currentTest.GetCorrectValue();
        ChosenValue = (CorrectChosen) ? CorrectValue : currentTest.GetSelectedValue();
        CorrectAngle = currentTest.GetCorrectRotation();
        ChosenAngle = currentTest.GetSelectedRotation();
        TimeAfterObserved = currentTest.TimeAfterObserved;
        MinValue = currentTest.MinValue;
        MaxValue = currentTest.MaxValue;
    }

}
