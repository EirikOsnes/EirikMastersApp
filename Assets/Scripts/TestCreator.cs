using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestCreator : MonoBehaviour
{

    public enum FieldOfView
    {
        Narrow,
        Full
    }

    public enum TestType
    {
        Height,
        Colour
    }

    public float narrowFOV = 90f;
    public int numOfBuildings = 8;
    public float heightOfBuildings = 50f; //Default height, for colour tests
    public float greyScaleColour = 80f; //Default colour, for height tests
    [Range (0,1)]
    public float spread = 0.2f; //Scaling the distribution away from the right answer.
    public FieldOfView fieldOfView = FieldOfView.Full;
    public TestType testType = TestType.Height;
    public float distance = 30f; //Meters away from observation point.
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

    public void CreateBuildings()
    {
        CreateTest();
    }

    private void SetRaycasterValues(GameObject go)
    {
        Canvas canvas = go.GetComponentInChildren<Canvas>();
        OVRRaycaster raycaster = go.GetComponentInChildren<OVRRaycaster>();
        if (canvas) canvas.worldCamera = eventCamera;
        if (raycaster) raycaster.pointer = laserPointer;
    }

    private void CreateTest()
    {
        GameObject container = new GameObject();
        container.transform.parent = this.transform;
        if (randomizeQuadrant) quadrant = Random.Range(1, 5);
        float degrees = FOVToFloat(fieldOfView);
        float degreesBetweenBuildings = (fieldOfView == FieldOfView.Narrow) ? degrees / (numOfBuildings - 1) : degrees / numOfBuildings;
        float currentRotation = CalculateStartRotation(degreesBetweenBuildings);
        int correctIndex = GenerateIndex();
        Debug.Log("Correct Index: " + correctIndex);
        GameObject correct;

        float[] values = (evenDistance) ? GenerateEvenValues(correctIndex) : GenerateValues(correctIndex);
        for (int i = 0; i < numOfBuildings; i++)
        {
            Vector3 spawnDirection = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
            GameObject go = Instantiate(buildingPrefab, new Vector3(spawnDirection.x * distance, 0, spawnDirection.z * distance), Quaternion.Euler(0, currentRotation, 0), container.transform);
            go.AddComponent<Building>();
            go.transform.localScale = new Vector3(1, (testType == TestType.Height) ? values[i] : heightOfBuildings, 1);
            currentRotation += degreesBetweenBuildings;

            if (testType == TestType.Colour || stripColours)
            {
                Renderer renderer = go.GetComponentInChildren<Renderer>();
                Material[] mats = new Material[renderer.sharedMaterials.Length];
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

            SetRaycasterValues(go);
            if (i == correctIndex) correct = go;
        }

        Test testComponent = container.AddComponent<Test>();
        testComponent.TestType = testType;
        testComponent.SetFOV(fieldOfView, narrowFOV);
        testComponent.generateID();
        container.name = testComponent.ID;
        testComponent.SetCorrect(container.GetComponentsInChildren<Building>()[correctIndex].gameObject);
        testComponent.MinValue = values.Min();
        testComponent.MaxValue = values.Max();
        testComponent.Spread = spread;
        testComponent.distance = distance;
        testComponent.Quadrant = quadrant;
    }

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

    private float CalculateStartRotation(float degsBetweenBuildings)
    {

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

    public float FOVToFloat(FieldOfView fov)
    {
        return (fov == FieldOfView.Narrow) ? narrowFOV : 360f;
    }
}
