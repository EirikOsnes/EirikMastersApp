using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void createBuildings()
    {
        if (testType == TestType.Height) createHeightTest();
        if (testType == TestType.Colour) createColourTest();
    }

    private void createHeightTest()
    {
        GameObject container = new GameObject("HeightTest");
        container.transform.parent = this.transform;
        float degrees = FOVToFloat(fieldOfView);
        float currentRotation = -degrees / 2f;
        Vector3 spawnDirection = Vector3.forward;
        float degreesBetweenBuildings = degrees / numOfBuildings;

        float[] heights = generateHeights();
        for (int i = 0; i < numOfBuildings; i++)
        {
            spawnDirection = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
            GameObject go = Instantiate(buildingPrefab, new Vector3(spawnDirection.x * distance, 0, spawnDirection.z * distance), Quaternion.Euler(0, currentRotation, 0), container.transform);
            go.transform.localScale = new Vector3(1, heights[i], 1);
            currentRotation += degreesBetweenBuildings;
        }
    }

    private void createColourTest()
    {
        GameObject container = new GameObject("ColourTest");
        container.transform.parent = this.transform;
        float degrees = FOVToFloat(fieldOfView);
        float currentRotation = -degrees / 2f;
        Vector3 spawnDirection = Vector3.forward;
        float degreesBetweenBuildings = degrees / numOfBuildings;

        float[] colours = generateColours();
        for (int i = 0; i < numOfBuildings; i++)
        {
            spawnDirection = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
            GameObject go = Instantiate(buildingPrefab, new Vector3(spawnDirection.x * distance, 0, spawnDirection.z * distance), Quaternion.Euler(0, currentRotation, 0), container.transform);
            go.transform.localScale = new Vector3(1, heightOfBuildings, 1);
            currentRotation += degreesBetweenBuildings;

            Renderer renderer = go.GetComponentInChildren<Renderer>();
            Material[] mats = new Material[renderer.sharedMaterials.Length];
            for (int j = 0; j < renderer.sharedMaterials.Length; j++)
            {
                Material myMaterial = new Material(Shader.Find("Standard"));
                myMaterial.color = new Color(colours[i]/255, colours[i]/255, colours[i]/255);
                mats[j] = myMaterial;
            }
            renderer.materials = mats;

        }
    }

    private float[] generateColours()
    {
        //TODO: This might have to be made better, maybe normalised sampling? Also need forced spread among all quadrants.
        float[] retVals = new float[numOfBuildings];

        int truePosition = Random.Range(0, numOfBuildings);

        for (int i = 0; i < retVals.Length; i++)
        {
            retVals[i] = greyScaleColour + Random.Range(- 128 * spread, 128 * spread);
        }

        retVals[truePosition] = greyScaleColour;

        return retVals;
    }

    private float[] generateHeights()
    {
        //TODO: This might have to be made better, maybe normalised sampling? Also need forced spread among all quadrants.
        float[] retVals = new float[numOfBuildings];

        int truePosition = Random.Range(0, numOfBuildings);

        for (int i = 0; i < retVals.Length; i++)
        {
            retVals[i] = heightOfBuildings + Random.Range(-heightOfBuildings * spread, heightOfBuildings * spread);
        }

        retVals[truePosition] = heightOfBuildings;

        return retVals;
    }

    public float FOVToFloat(FieldOfView fov)
    {
        return (fov == FieldOfView.Narrow) ? narrowFOV : 360f;
    }
}
