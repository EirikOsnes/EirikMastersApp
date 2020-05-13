using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunTimeTestCreator: MonoBehaviour
{
    public List<TestParameters> testParameters;
    public TestCreator testCreator;
    private List<TestSet> testSets = new List<TestSet>();

    public List<TestSet> TestSets { get => testSets; set => testSets = value; }

    //TODO: Get access to testsets in runner

    // Use this for initialization
    public void CreateTests()
    {
        if (!testCreator) testCreator = FindObjectOfType<TestCreator>();
        CreateAllTests();
    }

    private void CreateAllTests()
    {
        foreach (TestParameters parameters in testParameters)
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
                else
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

    // Update is called once per frame
    void Update()
    {

    }
}
