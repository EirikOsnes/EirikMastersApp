using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestRunner : MonoBehaviour
{

    List<Test> tests;
    Logger logger;
    Test currentTest;
    public OVRInput.RawButton lockinButton;
    OVRScreenFade screenFade;
    delegate void myMethod(Test t);
    public TestPass testPass;

    // Start is called before the first frame update
    void Start()
    {
        testPass = new TestPass();
        tests = GetAllTests();
        foreach (Test test in tests)
        {
            test.gameObject.SetActive(false);
        }
        screenFade = FindObjectOfType<OVRScreenFade>();
        if (tests.Count != 0)
        {
            StartCoroutine(WaitThenActivate(0f, tests[0], prepareTest, startTest, false));
        }
        logger = GetComponent<Logger>();
    }

    // Update is called once per frame
    void Update()
    {
        //Lock in the answer and transition to the next test.
        if (OVRInput.GetUp(lockinButton))
        {
            endTest();
            if (tests.Count > 0)
            {
                myMethod method = prepareTest;
                myMethod secondMethod = startTest;
                StartCoroutine(WaitThenActivate(0.5f, tests[UnityEngine.Random.Range(0, tests.Count)], method, startTest));
            } else
            {
                endTestPass();
            }
        }

        //try
        //{
        //    logger.ClearLog();
        //    logger.Log("Time used: " + currentTest.GetCurrentTime());
        //    logger.Log("Time used final: " + currentTest.TimeUsed);
        //    logger.Log("Since Observed: " + currentTest.GetCurrentTimeSinceObserved());
        //    logger.Log("Selected value: " + currentTest.GetSelectedValue());
        //    logger.Log("Selected Rotation: " + currentTest.GetSelectedRotation());
        //    logger.Log("Aggregated Rotation: " + currentTest.AggregatedRotation);
        //    logger.Log("Degrees Used: " + currentTest.GetDegreesUsed());
        //    float currentRotation = currentTest.__GetCurrentRotation();
        //    logger.Log("Current: " + currentRotation);
        //    logger.Log("Left: " + currentTest.leftAngle + " | Right: " + currentTest.rightAngle);
        //    logger.Log("Number of tests: " + tests.Count);
        //}
        //catch (Exception e)
        //{
        //    logger.Log(e.Message);
        //    logger.Log(e.StackTrace);
        //}
    }

    void endTest()
    {
        currentTest.EndTimer();
        testPass.tests.Add(new TestData(currentTest));
        tests.Remove(currentTest);
    }

    void endTestPass()
    {
        logger.WriteTestToFile(testPass);
    }

    void prepareTest(Test test)
    {
        if (currentTest != null)
        {
            currentTest.gameObject.SetActive(false);
        }
        test.gameObject.SetActive(true);
        currentTest = test;
    }

    void startTest(Test test)
    {
        test.StartTest();
    }

    List<Test> GetAllTests()
    {
        List<Test> objectsInScene = new List<Test>();

        foreach (Test t in Resources.FindObjectsOfTypeAll(typeof(Test)) as Test[])
        {
            if (/*!EditorUtility.IsPersistent(t.transform.root.gameObject) &&*/ !(t.hideFlags == HideFlags.NotEditable || t.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(t);
        }

        return objectsInScene;
    }

    IEnumerator WaitThenActivate(float waitTime, Test test, myMethod method = null, myMethod secondMethod = null, bool fadeout = true)
    {
        if(fadeout) screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        if (method != null) method(test);
        yield return new WaitForSeconds(waitTime);
        screenFade.FadeIn();
        if (secondMethod != null) secondMethod(test);
    }

}
