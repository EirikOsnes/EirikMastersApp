using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestRunner : MonoBehaviour
{

    public enum RunState
    {
        NotBegun,
        RealValue,
        TestRunning,
        Finished
    }

    List<Test> tests;
    Logger logger;
    Test currentTest;
    public OVRInput.RawButton lockinButton;
    OVRScreenFade screenFade;
    delegate void myMethod(Test t);
    public TestPass testPass;
    private SelectionHandler selectionHandler;
    private GameObject realVal;
    private RunState state = RunState.NotBegun;

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
        selectionHandler = gameObject.GetComponent<SelectionHandler>();
        logger = GetComponent<Logger>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            // LogDebugInfo();

            if (state == RunState.TestRunning)
            {
                //Lock in the answer and transition to the next test.
                if (OVRInput.GetUp(lockinButton))
                {
                    EndTest();
                    if (tests.Count > 0)
                    {
                        StartCoroutine(WaitThenActivate(0.5f, tests[UnityEngine.Random.Range(0, tests.Count)], ShowRealVal, null));
                    }
                    else
                    {
                        EndTestPass();
                    }
                }
            }

            if (state == RunState.RealValue)
            {
                if (OVRInput.GetUp(lockinButton))
                {
                    StartCoroutine(WaitThenActivate(0.5f, currentTest, PrepareTest, StartTest));
                }
            }

            if (state == RunState.NotBegun)
            {
                if (OVRInput.GetUp(lockinButton))
                {
                    if (tests.Count != 0)
                    {
                        StartCoroutine(WaitThenActivate(0f, tests[0], ShowRealVal));
                    }
                }
            }

            if (state == RunState.Finished)
            {

            }
        }
        catch (Exception e)
        {
            logger.Log("Exception caught: " + e.GetType().Name);
            logger.Log(e.Message);
            logger.Log(e.StackTrace);
        }

    }

    void LogDebugInfo()
    {
        logger.ClearLog();
        logger.Log(state.ToString());
        logger.Log("Time used: " + currentTest.GetCurrentTime());
        logger.Log("Time used final: " + currentTest.TimeUsed);
        logger.Log("Since Observed: " + currentTest.GetCurrentTimeSinceObserved());
        logger.Log("Selected value: " + currentTest.GetSelectedValue());
        logger.Log("Selected Rotation: " + currentTest.GetSelectedRotation());
        logger.Log("Aggregated Rotation: " + currentTest.AggregatedRotation);
        logger.Log("Degrees Used: " + currentTest.GetDegreesUsed());
        float currentRotation = currentTest.__GetCurrentRotation();
        logger.Log("Current: " + currentRotation);
        logger.Log("Left: " + currentTest.leftAngle + " | Right: " + currentTest.rightAngle);
        logger.Log("Number of tests: " + tests.Count);
    }

    void EndTest()
    {
        currentTest.EndTimer();
        logger.Log(currentTest.ToString());
        testPass.tests.Add(new TestData(currentTest));
        tests.Remove(currentTest);
    }

    void EndTestPass()
    {
        logger.WriteTestToFile(testPass);
        state = RunState.Finished;
    }

    void ShowRealVal(Test test)
    {
        selectionHandler.DestroySelector();
        if (currentTest != null)
        {
            currentTest.gameObject.SetActive(false);
        }
        currentTest = test;
        realVal = Instantiate(test.GetCorrect(), new Vector3(0, 0, test.distance), Quaternion.identity);
        state = RunState.RealValue;
    }

    void PrepareTest(Test test)
    {
        if (realVal != null) Destroy(realVal);
        test.gameObject.SetActive(true);
    }

    void StartTest(Test test)
    {
        test.StartTest();
        state = RunState.TestRunning;
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
