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

    public float narrowFOV = 90f;
    public float numOfBuildings = 8f;
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
        for (int i = 0; i < numOfBuildings; i++)
        {
            spawnDirection = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
            Instantiate(buildingPrefab, new Vector3(spawnDirection.x * distance, 0, spawnDirection.z * distance), Quaternion.Euler(0, currentRotation, 0), container.transform);
            currentRotation += degreesBetweenBuildings;
        }
    }

    public float FOVToFloat(FieldOfView fov)
    {
        return (fov == FieldOfView.Narrow) ? narrowFOV : 360f;
    }
}
