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
    private OVRCameraRig cameraRig;
    private GameObject playerController;

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
        cameraRig = FindObjectOfType<OVRCameraRig>();
        playerController = GameObject.Find("OVRPlayerController");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {

            if (state == RunState.TestRunning)
            {
                //Lock in the answer and transition to the next test.
                if (OVRInput.GetUp(lockinButton))
                {
                    EndTest();
                    if (tests.Count > 0)
                    {
                        StartCoroutine(WaitThenActivate(0.5f, GetNextTest(), ShowRealVal, null));
                    }
                    else
                    {
                        StartCoroutine(WaitThenActivate(0.5f, null, EndTestPass));
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

            LogDebugInfo();


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
        //logger.Log(state.ToString());
        //logger.Log("Time used: " + currentTest.GetCurrentTime());
        //logger.Log("Time used final: " + currentTest.TimeUsed);
        //logger.Log("Since Observed: " + currentTest.GetCurrentTimeSinceObserved());
        //logger.Log("Selected value: " + currentTest.GetSelectedValue());
        //logger.Log("Selected Rotation: " + currentTest.GetSelectedRotation());
        //logger.Log("Aggregated Rotation: " + currentTest.AggregatedRotation);
        //logger.Log("Degrees Used: " + currentTest.GetDegreesUsed());
        //float currentRotation = currentTest.__GetCurrentRotation();
        //logger.Log("Current: " + currentRotation);
        logger.Log("P-cont: " + playerController.transform.position.ToString());
        logger.Log("cea: " + cameraRig.centerEyeAnchor.position.ToString());
        logger.Log("cam: " + cameraRig.transform.position.ToString());
        //logger.Log("Left: " + currentTest.leftAngle + " | Right: " + currentTest.rightAngle);
        //logger.Log("Number of tests: " + tests.Count);
    }

    void EndTest()
    {
        currentTest.EndTimer();
        testPass.tests.Add(new TestData(currentTest));
        tests.Remove(currentTest);
    }

    void EndTestPass(Test test)
    {
        logger.WriteTestToFile(testPass);
        state = RunState.Finished;
        currentTest.gameObject.SetActive(false);
        selectionHandler.DestroySelector();
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
        ResetPosition();
    }

    void PrepareTest(Test test)
    {
        if (realVal != null) Destroy(realVal);
        test.gameObject.SetActive(true);
        ResetPosition();
    }

    void StartTest(Test test)
    {
        test.StartTest();
        state = RunState.TestRunning;
    }

    Test GetNextTest()
    {
        return tests[UnityEngine.Random.Range(0, tests.Count)];
    }

    void ResetPosition()
    {
        Transform cea = cameraRig.centerEyeAnchor;
        Transform cam = cameraRig.transform;

        float currentRotY = cea.eulerAngles.y;
        float difference = 0 - currentRotY;
        //cam.Rotate(0, difference, 0);
        playerController.transform.Rotate(0, difference, 0);

        Vector3 newPos = new Vector3(-playerController.transform.position.x, 0, -playerController.transform.position.z);
        cam.transform.position += newPos;
        //playerController.transform.position += newPos;

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
