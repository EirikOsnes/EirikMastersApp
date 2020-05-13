using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Creates and handles the functionality of interactions for the app.
/// </summary>
public class SelectionHandler : MonoBehaviour
{

    public Text text;
    //Marker prefab to add above the selected buildings.
    public GameObject selectionMarker;
    //The laser pointer to interact with the app.
    public LaserPointer pointer;
    //Current marker object.
    private GameObject selector;
    private Logger logger;

    //Instantiate the class.
    private void Start()
    {
        logger = GetComponent<Logger>();
        //assignButtonParameters();
    }

    /// <summary>
    /// Callback method for selecting building.
    /// </summary>
    /// <param name="go">The selected building.</param>
    public void OnBuildingClick(GameObject go)
    {
        //Click only if laserpointer is activated
        if (pointer.laserBeamBehavior == LaserPointer.LaserBeamBehavior.Off) return;

        GameObject newSelector = Instantiate(selectionMarker, go.transform.position + new Vector3(0, go.transform.lossyScale.y + 3, 0), Quaternion.Euler(-90, 0, 0));
        DestroySelector();
        selector = newSelector;
        Test myTest = go.GetComponentInParent<Test>();
        myTest.SetSelected(go);
    }

    /// <summary>
    /// Set up all button clicks for all buildings.
    /// </summary>
    public void assignButtonParameters()
    {
        List<Building> buildings = GetAllBuildings();
        GameObject[] buildingGos = new GameObject[buildings.Count];
        for (int i = 0; i < buildings.Count; i++)
        {
            buildingGos[i] = buildings[i].gameObject;
        }
        foreach (GameObject go in buildingGos)
        {
            Button[] btns = go.GetComponentsInChildren<Button>();
            foreach (Button btn in btns)
            {
                btn.onClick.AddListener(delegate { OnBuildingClick(go); });
            }
        }
    }

    /// <summary>
    /// Destroys any currently active selector.
    /// </summary>
    public void DestroySelector()
    {
        if (selector != null) Destroy(selector);
    }

    /// <summary>
    /// Checks to see if the laserpointer is currently active.
    /// </summary>
    /// <returns>True if laserpointer is active, false otherwise.</returns>
    public bool IsPointing()
    {
        return pointer.laserBeamBehavior == LaserPointer.LaserBeamBehavior.On;
    }

    /// <summary>
    /// Checks to see if any building is currently selected.
    /// </summary>
    /// <returns>True if a building is selected, fasle otherwise.</returns>
    public bool HasSelected()
    {
        return selector != null;
    }

    /// <summary>
    /// Finds all buildings for all tests in the scene.
    /// </summary>
    /// <returns>Returns a list of all buildings in the scene.</returns>
    List<Building> GetAllBuildings()
    {
        List<Building> objectsInScene = new List<Building>();

        foreach (Building b in Resources.FindObjectsOfTypeAll(typeof(Building)) as Building[])
        {
            if (/*!EditorUtility.IsPersistent(t.transform.root.gameObject) &&*/ !(b.hideFlags == HideFlags.NotEditable || b.hideFlags == HideFlags.HideAndDontSave))
                objectsInScene.Add(b);
        }

        return objectsInScene;
    }
}
