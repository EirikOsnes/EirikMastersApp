using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRunner : MonoBehaviour
{

    Test[] tests;
    Logger logger;
    Test currentTest;

    // Start is called before the first frame update
    void Start()
    {
        tests = FindObjectsOfType<Test>();
        if (tests.Length != 0)
        {
            tests[0].gameObject.SetActive(true);
            currentTest = tests[0];
            tests[0].StartTest();
            for (int i = 1; i < tests.Length; i++)
            {
                tests[i].gameObject.SetActive(false);
            }
        }

        logger = GetComponent<Logger>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            logger.ClearLog();
            logger.Log("Time used: " + currentTest.GetCurrentTime());
            logger.Log("Since Observed: " + currentTest.GetCurrentTimeSinceObserved());
            logger.Log("Selected value: " + currentTest.GetSelectedValue());
            logger.Log("Selected Rotation: " + currentTest.GetSelectedRotation());
            logger.Log("Aggregated Rotation: " + currentTest.AggregatedRotation);
            logger.Log("Degrees Used: " + currentTest.GetDegreesUsed());
            float currentRotation = currentTest.__GetCurrentRotation();
            logger.Log("Current: " + currentRotation);
            logger.Log("Left: " + currentTest.leftAngle + " | Right: " + currentTest.rightAngle);
        } catch (Exception e)
        {
            logger.Log(e.Message);
        }
    }

}
