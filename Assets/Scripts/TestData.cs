using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class has only purpose: To serve as a container for all the data that is to be logged for statistics.
/// </summary>
[Serializable]
public class TestData
{

    private Test currentTest;
    public string TestID;
    public float TimeUsed;
    public float AggregatedRotation; //Sum of all head rotation done
    public float DegreesUsed; //Total of the 360 degrees viewed
    public string TestType;
    public string FieldOfView;
    public bool CorrectChosen;
    public float CorrectValue;
    public float ChosenValue;
    public float CorrectAngle;
    public float ChosenAngle;
    public float TimeAfterObserved;
    public float TimeViewingTarget;
    public float MinValue;
    public float MaxValue;
    private Logger logger;
    

    public TestData (Test test, Logger logger = null)
    {
        this.logger = logger;
        currentTest = test;
        pollData();
    }

    /// <summary>
    /// Finds info from the live Test class and saves it in variables.
    /// </summary>
    private void pollData()
    {
        TestID = currentTest.ID;
        TimeUsed = currentTest.TimeUsed;
        TimeViewingTarget = currentTest.TimeViewingTarget;
        AggregatedRotation = currentTest.AggregatedRotation;
        DegreesUsed = currentTest.GetDegreesUsed();
        TestType = Enum.GetName(typeof(TestCreator.TestType) ,currentTest.TestType);
        FieldOfView = Enum.GetName(typeof(TestCreator.FieldOfView), currentTest.FieldOfView);
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
