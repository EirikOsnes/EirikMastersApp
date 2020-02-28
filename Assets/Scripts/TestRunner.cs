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

    // Start is called before the first frame update
    void Start()
    {
        tests = GetAllTests();
        screenFade = FindObjectOfType<OVRScreenFade>();
        if (tests.Count != 0)
        {
            startTest(0);
        }
        logger = GetComponent<Logger>();
    }

    // Update is called once per frame
    void Update()
    {
        //Lock in the answer and transition to the next test.
        if (OVRInput.GetUp(lockinButton))
        {
            startTest(UnityEngine.Random.Range(0, tests.Count));
        }

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
            logger.Log("Number of tests: " + tests.Count);
        }
        catch (Exception e)
        {
            logger.Log(e.Message);
        }
    }

    void startTest(int index)
    {
        StartCoroutine(WaitThenActivate(0.5f, index));
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

    IEnumerator WaitThenActivate(float waitTime, int index)
    {
        screenFade.FadeOut();
        yield return new WaitForSeconds(screenFade.fadeTime);
        tests[index].gameObject.SetActive(true);
        currentTest = tests[index];
        tests[index].StartTest();
        for (int i = 0; i < tests.Count; i++)
        {
            if (i == index) continue;
            tests[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(waitTime);
        screenFade.FadeIn();
    }

}
