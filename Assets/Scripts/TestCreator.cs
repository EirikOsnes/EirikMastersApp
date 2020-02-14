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
    [Range (0,1)]
    public float spread = 0.2f; //Scaling the distribution away from the right answer.
    public FieldOfView fieldOfView = FieldOfView.Full;
    public float distance = 30f; //Meters away from oberservation point.
    public GameObject buildingPrefab;

    public void createBuildings()
    {
        GameObject container = new GameObject("Test");
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
