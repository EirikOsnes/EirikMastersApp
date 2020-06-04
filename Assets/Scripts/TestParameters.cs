using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Test Parameters", menuName = "ScriptableObjects/TestParameter", order = 1)]
public class TestParameters : ScriptableObject
{
    public TestCreator.TestType testType;
    public TestCreator.FieldOfView fov;
    public List<BaseTestValues> testValues;
    

    public float narrowFOV = 90f;
    public int numOfBuildings = 8;
    public float heightOfBuildings = 50f; //Default height, for colour tests
    public float greyScaleColour = 80f; //Default colour, for height tests
    public float distance = 60f; //Meters away from observation point.
    public GameObject buildingPrefab;
    //public Camera eventCamera;
    //public GameObject laserPointer;
    public bool stripColours;
    public bool allQuadrants = true; //Create test for all quadrants.
    public bool narrowOncePerQuadrant = false; //Create a narrow test for each quadrant.
    public bool evenDistance = true;

    [Range(1, 4)]
    public int quadrant = 1;
    public bool randomizeQuadrant = false;

    [System.Serializable]
    public class BaseTestValues
    {
        public float minValue;
        public float maxValue;
    }

}
