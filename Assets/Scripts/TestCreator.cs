using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class to create a Test
/// </summary>
public class TestCreator : MonoBehaviour
{

    /// <summary>
    /// Available Fields of view.
    /// </summary>
    public enum FieldOfView
    {
        Narrow,
        Full,
        Both
    }

    /// <summary>
    /// Available Test Types
    /// </summary>
    public enum TestType
    {
        Height,
        Colour,
        Distance
    }

    public float narrowFOV = 90f;
    public int numOfBuildings = 8;
    public float heightOfBuildings = 50f; //Default height, for colour tests
    public float greyScaleColour = 80f; //Default colour, for height tests
    public FieldOfView fieldOfView = FieldOfView.Full;
    public TestType testType = TestType.Height;
    public float distance = 60f; //Meters away from observation point.
    public GameObject buildingPrefab;
    public Camera eventCamera;
    public GameObject laserPointer;
    public bool stripColours;
    [Range(1,4)]
    public int quadrant = 1;
    public bool randomizeQuadrant = false;
    public float minValue = 20f;
    public float maxValue = 60f;
    public bool evenDistance = false;
    public int testSet = 0;

    /// <summary>
    /// Public callback class for use in editor.
    /// </summary>
    /// <returns>The Gameobjects containing all buildings and the corresponding tests. If "Both", Narrow goes first.</returns>
    public List<GameObject> CreateBuildings()
    {
        if (!eventCamera) eventCamera = Camera.main;
        if (!laserPointer) laserPointer = GameObject.FindWithTag("Laser Pointer");
        List<GameObject> tests = new List<GameObject>();

        if(fieldOfView == FieldOfView.Both)
        {
            fieldOfView = FieldOfView.Narrow;
            tests.Add(CreateTest());
            fieldOfView = FieldOfView.Full;
            tests.Add(CreateTest());
        } else
        {
            tests.Add(CreateTest());
        }
 
        return tests;
    }

    /// <summary>
    /// Sets up canvas in front of Buildings to interact with the pointer.
    /// </summary>
    /// <param name="go">Building to set up interaction.</param>
    private void SetRaycasterValues(GameObject go)
    {
        Canvas canvas = go.GetComponentInChildren<Canvas>();
        OVRRaycaster raycaster = go.GetComponentInChildren<OVRRaycaster>();
        if (canvas) canvas.worldCamera = eventCamera;
        if (raycaster) raycaster.pointer = laserPointer;
    }

    /// <summary>
    /// Creates a Test based on the variables currently in the TestCreator.
    /// </summary>
    private GameObject CreateTest()
    {
        //Container for all buildings.
        GameObject container = new GameObject();
        container.transform.parent = this.transform;

        //Determine quadrant of correct answer if random.
        if (randomizeQuadrant) quadrant = Random.Range(1, 5);

        //Determine degrees between buildings, and rotation for first.
        float degrees = FOVToFloat(fieldOfView);
        float degreesBetweenBuildings = (fieldOfView == FieldOfView.Narrow) ? degrees / (numOfBuildings - 1) : degrees / numOfBuildings;
        float currentRotation = CalculateStartRotation(degreesBetweenBuildings);

        //Create index for correct answer.
        int correctIndex = GenerateIndex();
        GameObject correct;

        //Generate determining values for all buildings.
        float[] values = (evenDistance) ? GenerateEvenValues(correctIndex) : GenerateValues(correctIndex);

        //Create all buildings.
        for (int i = 0; i < numOfBuildings; i++)
        {
            //Direction of building.
            Vector3 spawnDirection = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
            float spawnDistance = (testType == TestType.Distance) ? values[i] : distance;
            //Instantiate building at given rotation and distance.
            GameObject go = Instantiate(buildingPrefab, new Vector3(spawnDirection.x * spawnDistance, 0, spawnDirection.z * spawnDistance), Quaternion.Euler(0, currentRotation, 0), container.transform);
            go.AddComponent<Building>();
            //Scale building size.
            go.transform.localScale = new Vector3(1, (testType == TestType.Height) ? values[i] : heightOfBuildings, 1);
            //Increment rotation
            currentRotation += degreesBetweenBuildings;

            //Set colour of building, stripping any texture from the prefab.
            if (testType == TestType.Colour || stripColours)
            {
                //Finds Renderer component of building, containing all visualisation parameters.
                Renderer renderer = go.GetComponentInChildren<Renderer>();
                Material[] mats = new Material[renderer.sharedMaterials.Length];

                //Replaces all materials of the prefab with a solid greyscale colour.
                for (int j = 0; j < renderer.sharedMaterials.Length; j++)
                {
                    Material myMaterial = new Material(Shader.Find("Standard"));
                    if (testType == TestType.Colour)
                    {
                        myMaterial.color = new Color(values[i] / 255, values[i] / 255, values[i] / 255);
                    } else
                    {
                        myMaterial.color = new Color(greyScaleColour / 255, greyScaleColour / 255, greyScaleColour / 255);
                    }
                    mats[j] = myMaterial;
                }
                renderer.materials = mats;
            }

            //Apply values so pointer can interact.
            SetRaycasterValues(go);
            //Set building as correct, if index is correct.
            if (i == correctIndex) correct = go;
        }

        //Create Test class containing the created buildings.
        Test testComponent = container.AddComponent<Test>();
        testComponent.TestType = testType;
        testComponent.SetFOV(fieldOfView, narrowFOV);
        testComponent.SetCorrect(container.GetComponentsInChildren<Building>()[correctIndex].gameObject);
        testComponent.MinValue = values.Min();
        testComponent.MaxValue = values.Max();
        testComponent.distance = distance;
        testComponent.Quadrant = quadrant;
        testComponent.testSet = testSet;
        testComponent.numberOfBuildings = numOfBuildings;
        testComponent.generateID();
        container.name = testComponent.ID;

        return container;
    }

    /// <summary>
    /// Generates determining values for all buildings, randomly sampled between given min and max values.
    /// </summary>
    /// <param name="pos">Position of correct answer.</param>
    /// <returns>Array of values for all buildings.</returns>
    private float[] GenerateValues(int pos)
    {
        float[] retVals = new float[numOfBuildings];

        if(testType == TestType.Colour) { 
            for (int i = 0; i < retVals.Length; i++)
            {
                //retVals[i] = greyScaleColour + Random.Range(-128 * spread, 128 * spread);
                retVals[i] = Random.Range(minValue, maxValue);
            }

            //retVals[pos] = greyScaleColour;
        } else
        {
            for (int i = 0; i < retVals.Length; i++)
            {
                //retVals[i] = heightOfBuildings + Random.Range(-heightOfBuildings * spread, heightOfBuildings * spread);
                retVals[i] = Random.Range(minValue, maxValue);
            }

            //retVals[pos] = heightOfBuildings;
        }

        return retVals;
    }

    /// <summary>
    /// Generates determining values for all buildings, evenly spaced between given min and max values.
    /// Order is randomised, 
    /// </summary>
    /// <param name="pos">Position of correct answer.</param>
    /// <returns>Array of values for all buildings.</returns>
    private float[] GenerateEvenValues(int pos)
    {
        float stepSize = (maxValue - minValue) / (numOfBuildings - 1);

        Stack<float> stack = new Stack<float>();
        for (int i = 0; i < numOfBuildings; i++)
        {
            stack.Push(minValue + i * stepSize);
        }
        var newStack = stack.OrderBy(x => Random.value);

        return newStack.ToArray();
    }

    /// <summary>
    /// Randomly generates index for a correct answer.
    /// </summary>
    /// <returns>Index of correct answer.</returns>
    private int GenerateIndex()
    {
        //No natural quadrants allowed
        if (fieldOfView == FieldOfView.Narrow || numOfBuildings % 4 != 0) return Random.Range(0, numOfBuildings);
        int buildingsPerQuad = (int)System.Math.Ceiling(numOfBuildings / 4f);
        //No overlap between quadrants
        if (buildingsPerQuad % 2 == 1)
        {
            int indexInQuad = Random.Range(0, buildingsPerQuad);
            return (quadrant - 1) * buildingsPerQuad + indexInQuad;
        }
        else
        {
            //There is overlap between quadrants, so the first and last building must have half the weight.
            int virtualBuildingNum = (buildingsPerQuad - 2 + 1 /*Because of one added from overlap*/) * 2 + 2;
            int virtualIndex = Random.Range(0, virtualBuildingNum);
            int indexInQuad;
            if (virtualIndex == 0) indexInQuad = 0;
            else if (virtualIndex == virtualBuildingNum - 1) indexInQuad = buildingsPerQuad - 1 + 1;
            else
            {
                indexInQuad = (virtualIndex + 1) / 2;
            }

            if (quadrant == 4 && indexInQuad == buildingsPerQuad) return 0; //Randomising the first position from the 4th quad.

            return (quadrant - 1) * buildingsPerQuad + indexInQuad;
        }
    }

    /// <summary>
    /// Calculate rotation for the first building.
    /// </summary>
    /// <param name="degsBetweenBuildings">Degrees between buildings.</param>
    /// <returns>Rotation of the first building in degrees.</returns>
    private float CalculateStartRotation(float degsBetweenBuildings)
    {
        //If field of view is Narrow, return leftmost angle in fov.
        if (fieldOfView == FieldOfView.Narrow || numOfBuildings % 4 != 0) return -narrowFOV / 2f;

        int buildingsPerQuad = (int)System.Math.Ceiling(numOfBuildings / 4f);

        //No overlap between quadrants
        if (buildingsPerQuad % 2 == 1)
        {            
            return -(buildingsPerQuad/2)*degsBetweenBuildings;
        }
        else
        {
            return -((buildingsPerQuad + 1) / 2) * degsBetweenBuildings;
        }
    }

    /// <summary>
    /// Translate FieldOfView to degrees.
    /// </summary>
    /// <param name="fov">Field of View</param>
    /// <returns>Angle in degrees</returns>
    public float FOVToFloat(FieldOfView fov)
    {
        return (fov == FieldOfView.Narrow) ? narrowFOV : 360f;
    }
}
