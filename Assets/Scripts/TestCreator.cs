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
    public float heightOfBuildings = 50f; //Correct answer, in meters
    public float greyScaleColour = 80f; //Correct answer, only for Colour TestType
    [Range (0,1)]
    public float spread = 0.2f; //Scaling the distribution away from the right answer.
    public FieldOfView fieldOfView = FieldOfView.Full;
    public TestType testType = TestType.Height;
    public float distance = 30f; //Meters away from observation point.
    public GameObject buildingPrefab;
    public Camera eventCamera;
    public GameObject laserPointer;
    public bool stripColours;

    public void createBuildings()
    {
        createTest();
    }

    private void setRaycasterValues(GameObject go)
    {
        Canvas canvas = go.GetComponentInChildren<Canvas>();
        OVRRaycaster raycaster = go.GetComponentInChildren<OVRRaycaster>();
        if (canvas) canvas.worldCamera = eventCamera;
        if (raycaster) raycaster.pointer = laserPointer;
    }

    private void createTest()
    {
        GameObject container = new GameObject();

        container.transform.parent = this.transform;
        float degrees = FOVToFloat(fieldOfView);
        float currentRotation = -degrees / 2f;
        float degreesBetweenBuildings = degrees / numOfBuildings;
        int correctIndex = 0;

        float[] values = generateValues(ref correctIndex);
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

            setRaycasterValues(go);
        }

        Test testComponent = container.AddComponent<Test>();
        testComponent.TestType = testType;
        testComponent.setFOV(fieldOfView, degrees);
        testComponent.generateID();
        container.name = testComponent.ID;
        testComponent.SetCorrect(container.GetComponentsInChildren<Building>()[correctIndex].gameObject);
        testComponent.MinValue = values.Min();
        testComponent.MaxValue = values.Max();
        testComponent.Spread = spread;
    }

    

    private float[] generateValues(ref int pos)
    {
        //TODO: This might have to be made better, maybe normalised sampling? Also need forced spread among all quadrants.
        float[] retVals = new float[numOfBuildings];

        int truePosition = Random.Range(0, numOfBuildings);

        if(testType == TestType.Colour) { 
            for (int i = 0; i < retVals.Length; i++)
            {
                retVals[i] = greyScaleColour + Random.Range(-128 * spread, 128 * spread);
            }

            retVals[truePosition] = greyScaleColour;
        } else
        {
            for (int i = 0; i < retVals.Length; i++)
            {
                retVals[i] = heightOfBuildings + Random.Range(-heightOfBuildings * spread, heightOfBuildings * spread);
            }

            retVals[truePosition] = heightOfBuildings;
        }

        return retVals;
    }

    public float FOVToFloat(FieldOfView fov)
    {
        return (fov == FieldOfView.Narrow) ? narrowFOV : 360f;
    }
}
