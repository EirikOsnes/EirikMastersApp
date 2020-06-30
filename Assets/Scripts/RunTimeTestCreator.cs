using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunTimeTestCreator: MonoBehaviour
{
    public List<TestParameters> testParameters;
    public TestCreator testCreator;
    private List<TestSet> testSets = new List<TestSet>();

    public List<TestSet> TestSets { get => testSets; set => testSets = value; }

    /// <summary>
    /// Create tests in runtime.
    /// </summary>
    /// <param name="testParameters">The test parameters to create tests for.</param>
    /// <returns>A list of TestSets containing all the tests.</returns>
    public List<TestSet> CreateTests(List<TestParameters> testParameters = null)
    {
        if (!testCreator) testCreator = FindObjectOfType<TestCreator>();
        TestSets = new List<TestSet>();
        CreateAllTests(testParameters);
        return testSets;
    }

    /// <summary>
    /// Creates all tests defined by the test parameters
    /// </summary>
    /// <param name="testParameters">Test Parameters defining the tests to be created</param>
    private void CreateAllTests(List<TestParameters> testParameters = null)
    {

        foreach (TestParameters parameters in (testParameters != null) ? testParameters : this.testParameters)
        {

            SetTestCreatorValues(parameters);


            for (int i = 0; i < parameters.testValues.Count; i++)
            {
                TestSet myTestSet = new TestSet();
                myTestSet.testType = parameters.testType;

                testCreator.minValue = parameters.testValues[i].minValue;
                testCreator.maxValue = parameters.testValues[i].maxValue;

                int[] quadrants = { parameters.quadrant };

                if (parameters.allQuadrants) {
                    quadrants = new int[] { 1, 2, 3, 4 };
                    testCreator.randomizeQuadrant = false;
                }

                if (parameters.fov == TestCreator.FieldOfView.Both)
                {

                    if(parameters.narrowOncePerQuadrant || quadrants.Length == 1)
                    {
                        for (int q = 0; q < quadrants.Length; q++)
                        {
                            testCreator.fieldOfView = TestCreator.FieldOfView.Both;
                            testCreator.quadrant = quadrants[q];
                            List<GameObject> bothTests = testCreator.CreateBuildings();
                            myTestSet.narrowTests.Add(bothTests[0]);
                            myTestSet.fullTests.Add(bothTests[1]);
                        }
                    } else
                    {
                        testCreator.fieldOfView = TestCreator.FieldOfView.Narrow;
                        myTestSet.narrowTests.AddRange(testCreator.CreateBuildings());

                        testCreator.fieldOfView = TestCreator.FieldOfView.Full;
                        for (int q = 0; q < quadrants.Length; q++)
                        {
                            testCreator.quadrant = quadrants[q];
                            myTestSet.fullTests.AddRange(testCreator.CreateBuildings());
                        }
                    }

                }
                else //Only one FieldOfView type to be created
                {
                    for (int q = 0; q < quadrants.Length; q++)
                    {
                        testCreator.quadrant = quadrants[q];
                        CreateOneDimensionalTests(parameters, myTestSet);
                    }
                }

                testSets.Add(myTestSet);
            }
        }
    }

    /// <summary>
    /// Creates test for one FieldOfView type
    /// </summary>
    /// <param name="parameters">Test Parameters defining tests to create</param>
    /// <param name="myTestSet">Test set to add tests to</param>
    private void CreateOneDimensionalTests(TestParameters parameters, TestSet myTestSet)
    {
        if (parameters.fov == TestCreator.FieldOfView.Full)
        {
            testCreator.fieldOfView = parameters.fov;
            myTestSet.fullTests.AddRange(testCreator.CreateBuildings());
        }
        else
        {
            testCreator.fieldOfView = parameters.fov;
            myTestSet.narrowTests.AddRange(testCreator.CreateBuildings());
        }
    }

    /// <summary>
    /// Set up the TestCreator.
    /// </summary>
    /// <param name="parameters">The test parameters to set the TestCreator to.</param>
    private void SetTestCreatorValues(TestParameters parameters)
    {
        testCreator.testType = parameters.testType;
        testCreator.narrowFOV = parameters.narrowFOV;
        testCreator.numOfBuildings = parameters.numOfBuildings;
        testCreator.heightOfBuildings = parameters.heightOfBuildings;
        testCreator.greyScaleColour = parameters.greyScaleColour;
        testCreator.distance = parameters.distance;
        testCreator.buildingPrefab = parameters.buildingPrefab;
        testCreator.stripColours = parameters.stripColours;
        testCreator.evenDistance = parameters.evenDistance;
        testCreator.randomizeQuadrant = parameters.randomizeQuadrant;
    }
}
