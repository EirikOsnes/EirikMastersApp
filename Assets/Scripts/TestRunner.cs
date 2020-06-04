using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for running of tests.
/// </summary>
public class TestRunner : MonoBehaviour
{
    /// <summary>
    /// Current state of run.
    /// </summary>
    public enum RunState
    {
        SelectGroup,
        NotBegun,
        TutorialFinished,
        RealValue,
        TestRunning,
        Finished
    }

    List<Test> tests;
    List<Test> tutorialTests;
    List<Test> tutorialTestBackup;
    Logger logger;
    Test currentTest;
    public bool createTestAtRuntime = true;
    public OVRInput.RawButton lockinButton;
    public bool skippingAllowed = true;
    OVRScreenFade screenFade;
    delegate void myMethod(Test t);
    private TestPass testPass;
    private SelectionHandler selectionHandler;
    private GameObject realVal;
    private RunState state = RunState.SelectGroup;
    private OVRCameraRig cameraRig;
    private GameObject playerController;
    private GameObject A_Button_Tooltip;
    private GameObject X_Button_Tooltip;
    private GameObject R2_Button_Tooltip;
    private RunTimeTestCreator creator;
    public Canvas infoScreen;
    public Canvas groupSelectScreen;

    private bool tutorialEndIsHeld = false;
    private float tutorialEndStart = -1;
    private float timeToHold = 1;

    public bool narrowFirst = true;
    public bool randomiseSetOrder = false;
    public bool separateNarrowAndFull = true;
    public bool separateTestSets = true;
    public bool splitByTestType = true;
    public bool reverseTestTypeOrder = false;
    public bool nextVariablesShown = false;
    private int orderingNum = 0;

    public float exposureTimeLimit = 0;
    public float searchTimeLimit = 0;
    private float stateStartTime;

    public List<TestParameters> tutorialParameters;
    private bool tutorialActive = false;
    private bool transitioning = false;

    private DateTime startTime;

    // Start is called before the first frame update
    void Start()
    {
        logger = GetComponent<Logger>();
        Time.timeScale = 1.0f;
        testPass = new TestPass();
        creator = GameObject.FindObjectOfType<RunTimeTestCreator>();
        //SetUpTests();
        SetUpTutorialTests();
        logger.Log("tutorialActive: " + tutorialActive);
        logger.Log("tutorials count: " + tutorialTests.Count);
        DisableAllTests(GetAllTestsInScene());
        screenFade = FindObjectOfType<OVRScreenFade>();
        selectionHandler = gameObject.GetComponent<SelectionHandler>();
        cameraRig = FindObjectOfType<OVRCameraRig>();
        playerController = GameObject.Find("OVRPlayerController");
        A_Button_Tooltip = GameObject.Find("A_Button_Tooltip");
        X_Button_Tooltip = GameObject.Find("X_Button_Tooltip");
        R2_Button_Tooltip = GameObject.Find("R2_Button_Tooltip");
        DisableTooltips(R2: false, A: false);
        UpdateInfoText();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {

            if (transitioning) return;

            // Test currently running.
            if (state == RunState.TestRunning)
            {

                if(!tutorialActive && searchTimeLimit > 0 && (Time.time - stateStartTime) > searchTimeLimit)
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

                //Activate/Deactivate applicable tooltips.
                if (selectionHandler.IsPointing()) A_Button_Tooltip.SetActive(true);
                else A_Button_Tooltip.SetActive(false);

                if (selectionHandler.HasSelected()) X_Button_Tooltip.SetActive(true);
                else X_Button_Tooltip.SetActive(false);

                //Lock in the answer and transition to the next test.
                if (OVRInput.GetUp(lockinButton) && (skippingAllowed || selectionHandler.HasSelected()))
                {
                    EndTest();

                    if (tutorialActive)
                    {

                        if(tutorialTests.Count > 0)
                        {
                            StartCoroutine(WaitThenActivate(0.5f, GetNextTest(), ShowRealVal, null));
                        }
                        else
                        {
                            StartCoroutine(WaitThenActivate(0.5f, null, EndTutorial));
                        }

                    }

                    else
                    {
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
            }

            // Real value currently shown.
            if (state == RunState.RealValue)
            {

                if(!tutorialActive && exposureTimeLimit > 0 && (Time.time - stateStartTime) > exposureTimeLimit)
                {
                    StartCoroutine(WaitThenActivate(0.5f, currentTest, PrepareTest, StartTest));
                }

                //Change view to the running test.
                if (OVRInput.GetUp(lockinButton))
                {
                    StartCoroutine(WaitThenActivate(0.5f, currentTest, PrepareTest, StartTest));
                }
            }

            if (state == RunState.TutorialFinished)
            {
                //Start tutorial again
                if (OVRInput.GetUp(lockinButton))
                {
                    if (tutorialEndStart < 0) tutorialEndStart = Time.time;
                    float timeHeld = Time.time - tutorialEndStart;
                    if (timeHeld > timeToHold)
                    {
                        //End tutorial
                        tutorialActive = false;
                        StartCoroutine(WaitThenActivate(0.5f, GetNextTest(), ShowRealVal, null));
                    }
                    else
                    {
                        tutorialEndIsHeld = false;
                        StartCoroutine(WaitThenActivate(0.5f, GetNextTest(), ShowRealVal, null));
                    }
                }

                if (OVRInput.Get(lockinButton))
                {
                    if(!tutorialEndIsHeld)
                    {
                        tutorialEndIsHeld = true;
                        tutorialEndStart = Time.time;
                    }
                    logger.SetLog("Time Held: " + (Time.time - tutorialEndStart));
                }
                else
                {
                    tutorialEndIsHeld = false;
                }                
            }

            // Initial state still active, neither test nor real value yet shown.
            // Any tutorial will be started from this stage.
            if (state == RunState.NotBegun)
            {
                if (OVRInput.GetUp(lockinButton))
                {
                    if (tests.Count != 0)
                    {
                        StartCoroutine(WaitThenActivate(0f, GetNextTest(), ShowRealVal));
                        startTime = DateTime.Now;
                    }
                }
            }

            // Final state when all testpasses are completed. 
            // Any surveys or thank yous will be started from this stage.
            if (state == RunState.Finished)
            {

            }

            //LogDebugInfo();


        }
        catch (Exception e)
        {
            //Log any exception thrown.
            logger.gameObject.SetActive(true);
            logger.Log("Exception caught: " + e.GetType().Name);
            logger.Log(e.Message);
            logger.Log(e.StackTrace);
            throw e;
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
        //logger.Log("P-cont: " + playerController.transform.position.ToString());
        //logger.Log("cea: " + cameraRig.centerEyeAnchor.position.ToString());
        //logger.Log("cam: " + cameraRig.transform.position.ToString());
        //logger.Log("Left: " + currentTest.leftAngle + " | Right: " + currentTest.rightAngle);
        //logger.Log("Number of tests: " + tests.Count);
    }

    /// <summary>
    /// Disables tooltips.
    /// </summary>
    /// <param name="A">True if "A"-button tooltip is to be disabled.</param>
    /// <param name="R2">True if "R2"-button tooltip is to be disabled.</param>
    /// <param name="X">True if "X"-button tooltip is to be disabled.</param>
    private void DisableTooltips(bool A = true, bool R2 = true, bool X = true)
    {
        A_Button_Tooltip.SetActive(!A);
        R2_Button_Tooltip.SetActive(!R2);
        X_Button_Tooltip.SetActive(!X);
    }

    /// <summary>
    /// End the current test.
    /// </summary>
    void EndTest()
    {
        currentTest.EndTimer();
        if (!tutorialActive)
        {
            TestData currentTestData = new TestData(currentTest);
            testPass.tests.Add(currentTestData);
            logger.WriteTestToFile(currentTestData, startTime, currentTest.ID);
            tests.Remove(currentTest);
        } else
        {
            tutorialTests.Remove(currentTest);
        }
    }

    /// <summary>
    /// End the current TestPass, and write it to file.
    /// </summary>
    /// <param name="test">Leave as null</param>
    void EndTestPass(Test test = null)
    {
        logger.WriteTestPassToFile(testPass, startTime, "TestPass");
        state = RunState.Finished;
        currentTest.gameObject.SetActive(false);
        selectionHandler.DestroySelector();
        DisableTooltips();
        UpdateInfoText();
    }

    void EndTutorial(Test test = null)
    {
        logger.Log("Tutorial ended");
        state = RunState.TutorialFinished;
        currentTest.gameObject.SetActive(false);
        selectionHandler.DestroySelector();
        DisableTooltips(X: false);
        UpdateInfoText();
    }

    /// <summary>
    /// Display the correct value directly ahead.
    /// </summary>
    /// <param name="test">Test for which correct value is to be shown.</param>
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
        DisableTooltips(X: false);
        UpdateInfoText(test);
        stateStartTime = Time.time;
    }

    /// <summary>
    /// Clears the scene and activates the given test.
    /// </summary>
    /// <param name="test">Test to activate</param>
    void PrepareTest(Test test)
    {
        if (realVal != null) Destroy(realVal);
        test.gameObject.SetActive(true);
        ResetPosition();
    }

    /// <summary>
    /// Starts the test.
    /// </summary>
    /// <param name="test">Test to start.</param>
    void StartTest(Test test)
    {
        test.StartTest();
        state = RunState.TestRunning;
        DisableTooltips(R2: false);
        UpdateInfoText(test);
        stateStartTime = Time.time;
    }

    void GroupIsSelected(Test test = null)
    {
        try {
            state = RunState.NotBegun;
            DisableTooltips(X: false);
            UpdateInfoText();
            groupSelectScreen.gameObject.SetActive(false);
            SetUpTests();
            testPass.orderOption = orderingNum;
        }
        catch (Exception e)
        {
            logger.Log(e.ToString());
        }
        
    }

    /// <summary>
    /// Determine next test to run.
    /// </summary>
    /// <returns>The next test.</returns>
    Test GetNextTest()
    {
        if (tutorialActive && tutorialTestBackup.Count > 0)
        {
            if (tutorialTests.Count > 0) return tutorialTests[0];
            else
            {
                tutorialTests = new List<Test>(tutorialTestBackup);
                return tutorialTests[0];
            }
        }
        //TODO: This method can and should be improved upon to fit statistical methods.
        return tests[0];
    }

    private void SetUpTutorialTests()
    {
        if (tutorialParameters == null) return;
        List<TestSet> sets = creator.CreateTests(tutorialParameters);
        List<Test> tutorials = new List<Test>();
        foreach (TestSet set in sets)
        {
            tutorials.AddRange(TestListFromGameObjectList(set.narrowTests));
            tutorials.AddRange(TestListFromGameObjectList(set.fullTests));
        }
        tutorialTests = new List<Test>(tutorials);
        tutorialTestBackup = new List<Test>(tutorials);
        tutorialActive = true;
    }

    private void SetUpTests()
    {

        //TODO: Separate by TestType?

        List<TestSet> sets = GetTestSets();
        List<Test> tests = new List<Test>();

        if (splitByTestType)
        {
            List<TestSet>[] testTypeSets = new List<TestSet>[Enum.GetNames(typeof(TestCreator.TestType)).Length];
            for (int i = 0; i < testTypeSets.Length; i++)
            {
                testTypeSets[i] = new List<TestSet>();
            }
            foreach (TestSet testSet in sets)
            {
                testTypeSets[(int)testSet.testType].Add(testSet);
            }

            if (!reverseTestTypeOrder)
            {
                for (int i = 0; i < testTypeSets.Length; i++)
                {
                    tests.AddRange(GetTestOrder(testTypeSets[i]));
                }
            }
            else
            {
                for (int i = testTypeSets.Length - 1; i >= 0; i--)
                {
                    tests.AddRange(GetTestOrder(testTypeSets[i]));

                }
            }
        }

        else
        {
            tests.AddRange(GetTestOrder(sets));
        }

        this.tests = tests;
        selectionHandler.assignButtonParameters();
        DisableAllTests(GetAllTestsInScene());
    }

    private List<Test> GetTestOrder(List<TestSet> sets)
    {
        List<Test> tests = new List<Test>();
        //The order of the sets are random.
        if (randomiseSetOrder) sets.Shuffle();

        //Each test set should be completed before the next begins.
        if (separateTestSets)
        {
            for (int i = 0; i < sets.Count; i++)
            {
                List<GameObject> setTests = new List<GameObject>();
                //All Narrow tests should be completed before Full, or vice versa.
                if (separateNarrowAndFull)
                {
                    setTests.AddRange((narrowFirst) ? sets[i].narrowTests : sets[i].fullTests);
                    setTests.AddRange((narrowFirst) ? sets[i].fullTests : sets[i].narrowTests);
                }
                //Corresponding Narrow and Full tests are successive.
                else
                {
                    int narrow = sets[i].narrowTests.Count;
                    int full = sets[i].fullTests.Count;
                    for (int j = 0; j < Math.Max(narrow, full); j++)
                    {
                        //Narrow Tests go first
                        if (narrowFirst)
                        {
                            if (narrow > j) setTests.Add(sets[i].narrowTests[j]);
                            if (full > j) setTests.Add(sets[i].fullTests[j]);
                        }
                        //Full Tests go first
                        else
                        {
                            if (full > j) setTests.Add(sets[i].fullTests[j]);
                            if (narrow > j) setTests.Add(sets[i].narrowTests[j]);
                        }
                    }
                }
                tests.AddRange(TestListFromGameObjectList(setTests));
            }
        }
        //Separate tests from a test set might not be successive.
        else
        {
            List<GameObject> allTests = new List<GameObject>();

            if (separateNarrowAndFull)
            {
                List<GameObject> narrowTests = new List<GameObject>();
                List<GameObject> fullTests = new List<GameObject>();
                for (int i = 0; i < sets.Count; i++)
                {
                    narrowTests.AddRange(sets[i].narrowTests);
                    fullTests.AddRange(sets[i].fullTests);
                }
                //FIXME: Should narrow and full be shuffled?
                allTests.AddRange((narrowFirst) ? narrowTests : fullTests);
                allTests.AddRange((narrowFirst) ? fullTests : narrowTests);
            }
            // Unsupported, returns all tests in random order.
            else
            {
                for (int i = 0; i < sets.Count; i++)
                {
                    allTests.AddRange(sets[i].narrowTests);
                    allTests.AddRange(sets[i].fullTests);
                }
                allTests.Shuffle();
            }

            tests.AddRange(TestListFromGameObjectList(allTests));
        }

        return tests;
    }

    private void DisableAllTests(List<Test> tests)
    {
        foreach (Test test in tests)
        {
            test.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Resets the positon of the user to face forward and be located in the center of the test area.
    /// </summary>
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

    List<TestSet> GetTestSets()
    {
        if (createTestAtRuntime) return GetAllTestSetsFromRuntimeCreator();
        else
        {
            List<TestSet> sets = new List<TestSet>();
            List<Test> tests = GetAllTestsInScene();
            Dictionary<int, TestSet> dict = new Dictionary<int, TestSet>();
            foreach (Test test in tests)
            {
                TestSet mySet;
                if (dict.ContainsKey(test.testSet))
                {
                    mySet = dict[test.testSet];
                }
                else
                {
                    mySet = new TestSet();
                    dict.Add(test.testSet, mySet);
                }
                if(test.FieldOfView == TestCreator.FieldOfView.Full)
                {
                    mySet.fullTests.Add(test.gameObject);
                } else
                {
                    mySet.narrowTests.Add(test.gameObject);
                }
            }

            foreach (KeyValuePair<int,TestSet> pair in dict)
            {
                sets.Add(pair.Value);
            }

            return sets;
        }
    }

    List<TestSet> GetAllTestSetsFromRuntimeCreator()
    {
        return creator.CreateTests();
    }

    private void UpdateInfoText(Test test = null)
    {
        string infoString = "";
        infoScreen.gameObject.SetActive(true);
        if(state == RunState.SelectGroup)
        {
            infoScreen.gameObject.SetActive(false);
        }
        if (state == RunState.NotBegun)
        {
            infoString += "Thanks for partaking in this experiment. We will begin with a tutorial. To begin this tutorial, please press the X-button highlighted on your left controller";
        }
        if(state == RunState.RealValue)
        {
            if (tutorialActive)
            {
                infoString += "Tests consist of 2 stages. In this stage you are shown an object. In the next stage you are to locate this object among others.";
                infoString += "\nWhen you feel you are ready, press the X-button highlighted on you left controller to proceed to the second stage";
            }
            if (test != null)
            {
                infoString += "\n\nThis test will vary the " + test.TestType.ToString() + " of the objects.\n\nThe different objects will be placed " +
                    ((test.FieldOfView == TestCreator.FieldOfView.Full) ? "anywhere around you. Remember to look around!" : "within your field of view.");
            }
            if (!tutorialActive && !nextVariablesShown)
            {
                infoScreen.gameObject.SetActive(false);
            }
        }
        if(state == RunState.TestRunning)
        {
            if (tutorialActive)
            {
                infoString += "Here you are shown " + test.numberOfBuildings + " objects. Please select the one you think you saw in the previous view.";
                infoString += "\n\nAll controls available to you are highlighted on your controllers:";
                infoString += "\nTo select, activate the pointer on your right hand, point at the object of choice, and select it with the A-button.";
                infoString += "\nWhen selected, a marker will appear above the building, and the X-button on your left controller will prompt you to accept your selection.";
                infoString += "\nIf you change your mind, you can select a new object before pressing the X-button";
            } else
            {
                infoScreen.gameObject.SetActive(false);
            }
        }
        if(state == RunState.TutorialFinished)
        {
            infoString += "Well done, you have finished the tutorial. If you are unsure about any controls, please retake the tutorial. If you are ready, you can now proceed to the experiment.";
            infoString += "\n\nTo redo the tutorial, click the X-button highlighted on your left controller.";
            infoString += "\n\nTo continue to the experiment, press and HOLD the X-button highlighted on your left controller for 2 seconds.";
        }
        if(state == RunState.Finished)
        {
            infoString += "The experiment is finished.\n\nThank you for participating!";
        }

        
        Text infoText = infoScreen.transform.Find("Infotext").GetComponent<Text>();
        infoText.text = infoString;
    }

    /// <summary>
    /// Selects the ordering of tests based on the input number. Used as a button callback.
    /// 1: Narrow first, Height first.
    /// 2: Narrow first, Colour first.
    /// 3: Full first, Height first.
    /// 4: Full first, Colour first.
    /// </summary>
    /// <param name="buttonNum"></param>
    public void SelectOrderingByButton(int buttonNum)
    {
        orderingNum = buttonNum;
        switch (buttonNum)
        {
            case 1:
                narrowFirst = true;
                reverseTestTypeOrder = false;
                goto default;
            case 2:
                narrowFirst = true;
                reverseTestTypeOrder = true;
                goto default;
            case 3:
                narrowFirst = false;
                reverseTestTypeOrder = false;
                goto default;
            case 4:
                narrowFirst = false;
                reverseTestTypeOrder = true;
                goto default;
            default:
                StartCoroutine(WaitThenActivate(0.5f, null, GroupIsSelected, null));
                break;

        }
    }

    /// <summary>
    /// Get all the tests in the scene.
    /// </summary>
    /// <returns>All Tests in the Scene as a List.</returns>
    List<Test> GetAllTestsInScene()
    {
        List<Test> objectsInScene = new List<Test>();

        foreach (Test t in Resources.FindObjectsOfTypeAll(typeof(Test)) as Test[])
        {
            if (/*!EditorUtility.IsPersistent(t.transform.root.gameObject) &&*/ !(t.hideFlags == HideFlags.NotEditable || t.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(t);
        }

        return objectsInScene;
    }

    /// <summary>
    /// Fading transition between views.
    /// </summary>
    /// <param name="waitTime">Time spent with screen fully black, in seconds</param>
    /// <param name="test">Test methods are to be ran on.</param>
    /// <param name="method">Method ran after fadeout, but before fadein.</param>
    /// <param name="secondMethod">Method ran after fadein.</param>
    /// <param name="fadeout">True if view should fade out, false if cutting straight to black.</param>
    /// <returns></returns>
    IEnumerator WaitThenActivate(float waitTime, Test test, myMethod method = null, myMethod secondMethod = null, bool fadeout = true)
    {
        transitioning = true;
        if(fadeout) screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        if (method != null) method(test);
        yield return new WaitForSeconds(waitTime);
        screenFade.FadeIn();
        if (secondMethod != null) secondMethod(test);
        transitioning = false;
    }

    public List<Test> TestListFromGameObjectList(List<GameObject> gameObjects)
    {
        List<Test> tests = new List<Test>();
        foreach (GameObject gameObject in gameObjects)
        {
            tests.Add(gameObject.GetComponent<Test>());
        }
        return tests;
    }

}

static class Helpers
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
